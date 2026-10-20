[CmdletBinding()]
param(
    [string]$StagingPath = 'C:\BackServices\GenDb2.0_next',
    [string]$ProductionPath = 'C:\BackServices\GenDb2.0',
    [string]$ServiceName = 'GenDbGPAO',
    [string]$ServiceDisplayName = 'GenDb - Import GPAO',
    [string]$LegacyTaskName,
    [string]$LegacyTaskPath = '\',
    [int]$ServiceStartupTimeoutSeconds = 30
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$script:LegacyTasks = @()
$script:LegacyTasksToReEnable = @()
$script:ExistingService = $null
$script:ServiceWasRunning = $false
$script:ServiceCreated = $false
$script:BackupPath = $null
$script:DeploymentActivated = $false

function Write-Step {
    param([Parameter(Mandatory)][string]$Message)
    Write-Host "`n==> $Message" -ForegroundColor Cyan
}

function Assert-Administrator {
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = [Security.Principal.WindowsPrincipal]::new($identity)
    if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
        throw 'Ce script doit être exécuté dans un terminal PowerShell administrateur.'
    }
}

function Get-LegacyGenDbTask {
    if ($LegacyTaskName) {
        return @(Get-ScheduledTask -TaskName $LegacyTaskName -TaskPath $LegacyTaskPath)
    }

    $matches = @(
        Get-ScheduledTask | Where-Object {
            foreach ($action in $_.Actions) {
                $executeProperty = $action.PSObject.Properties['Execute']
                $argumentsProperty = $action.PSObject.Properties['Arguments']
                $execute = if ($executeProperty) { [string]$executeProperty.Value } else { '' }
                $arguments = if ($argumentsProperty) { [string]$argumentsProperty.Value } else { '' }
                if ($execute -like '*GenDb*' -or $arguments -like '*GenDb*') {
                    return $true
                }
            }
            return $false
        }
    )

    return $matches
}

function Assert-StagingContent {
    if (-not (Test-Path -LiteralPath $StagingPath -PathType Container)) {
        throw "Le dossier publié est introuvable : $StagingPath"
    }

    $requiredFiles = @('GenDb.exe', 'GenDb.dll', 'GenDb.runtimeconfig.json', 'appsettings.json')
    foreach ($file in $requiredFiles) {
        $path = Join-Path $StagingPath $file
        if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
            throw "Publication incomplète : $path est absent."
        }
    }

    $settingsPath = Join-Path $StagingPath 'appsettings.json'
    try {
        $settings = Get-Content -LiteralPath $settingsPath -Raw | ConvertFrom-Json
    }
    catch {
        throw "appsettings.json est invalide : $($_.Exception.Message)"
    }

    if (-not $settings.ConnectionStrings.ProductionDatabase) {
        throw 'La chaîne ConnectionStrings:ProductionDatabase est absente.'
    }

    $expectedCsvKeys = @(
        'Articles',
        'BillOfMaterials',
        'Users',
        'Operations',
        'ManufacturingOrders',
        'FirmwareVersions',
        'OrderLines'
    )

    foreach ($key in $expectedCsvKeys) {
        $path = $settings.CsvPaths.$key
        if ([string]::IsNullOrWhiteSpace($path)) {
            throw "CsvPaths:$key est absent de la configuration."
        }

        if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
            throw "Le fichier CSV configuré pour $key est introuvable : $path"
        }

        $file = Get-Item -LiteralPath $path
        if ($file.Length -eq 0) {
            throw "Le fichier CSV configuré pour $key est vide : $path"
        }

        Write-Host ("  {0,-22} {1:yyyy-MM-dd HH:mm:ss}  {2,12:N0} octets" -f $key, $file.LastWriteTime, $file.Length)
    }

    $processingRoot = 'C:\Data\GenDb\Processing'
    $stateFile = 'C:\Data\GenDb\last-successful-batch.json'
    $importServiceProperty = $settings.PSObject.Properties['ImportService']
    if ($importServiceProperty) {
        $configuredProcessingRoot = [string]$importServiceProperty.Value.ProcessingRoot
        $configuredStateFile = [string]$importServiceProperty.Value.StateFile
        if (-not [string]::IsNullOrWhiteSpace($configuredProcessingRoot)) {
            $processingRoot = $configuredProcessingRoot
        }
        if (-not [string]::IsNullOrWhiteSpace($configuredStateFile)) {
            $stateFile = $configuredStateFile
        }
    }

    $processingParent = Split-Path -Parent $processingRoot
    if (-not $processingParent) {
        throw "ImportService:ProcessingRoot est invalide : $processingRoot"
    }

    New-Item -ItemType Directory -Path $processingRoot -Force | Out-Null
    $stateDirectory = Split-Path -Parent $stateFile
    New-Item -ItemType Directory -Path $stateDirectory -Force | Out-Null

    $isSelfContained = Test-Path -LiteralPath (Join-Path $StagingPath 'hostfxr.dll')
    if (-not $isSelfContained) {
        $runtimeConfig = Get-Content -LiteralPath (Join-Path $StagingPath 'GenDb.runtimeconfig.json') -Raw | ConvertFrom-Json
        $runtimeOptions = $runtimeConfig.runtimeOptions
        $frameworkVersion = $null
        $singleFramework = $runtimeOptions.PSObject.Properties['framework']
        if ($singleFramework) {
            $frameworkVersion = [string]$singleFramework.Value.version
        }
        else {
            $frameworksProperty = $runtimeOptions.PSObject.Properties['frameworks']
            if ($frameworksProperty) {
                $netCoreFramework = $frameworksProperty.Value |
                    Where-Object { $_.name -eq 'Microsoft.NETCore.App' } |
                    Select-Object -First 1
                if ($netCoreFramework) {
                    $frameworkVersion = [string]$netCoreFramework.version
                }
            }
        }

        if ([string]::IsNullOrWhiteSpace($frameworkVersion)) {
            throw 'Impossible de déterminer la version .NET requise dans GenDb.runtimeconfig.json.'
        }

        $dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
        if (-not $dotnet) {
            throw ".NET $frameworkVersion n'est pas disponible et la publication n'est pas autonome."
        }

        $requiredMajor = ([Version]$frameworkVersion).Major
        $installedRuntimes = @(& dotnet --list-runtimes)
        if (-not ($installedRuntimes -match "^Microsoft\.NETCore\.App $requiredMajor\.")) {
            throw "Le runtime Microsoft.NETCore.App $requiredMajor.x n'est pas installé."
        }
    }
}

function Wait-ServiceRunning {
    param([Parameter(Mandatory)][string]$Name)

    $deadline = (Get-Date).AddSeconds($ServiceStartupTimeoutSeconds)
    do {
        Start-Sleep -Seconds 1
        $service = Get-Service -Name $Name -ErrorAction Stop
        if ($service.Status -eq 'Running') {
            return
        }
    } while ((Get-Date) -lt $deadline)

    throw "Le service $Name n'est pas passé à l'état Running dans le délai imparti."
}

function Restore-PreviousDeployment {
    Write-Warning 'La bascule a échoué. Tentative de retour arrière...'

    $service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
    if ($service -and $service.Status -ne 'Stopped') {
        Stop-Service -Name $ServiceName -Force -ErrorAction SilentlyContinue
    }

    if ($script:DeploymentActivated -and (Test-Path -LiteralPath $ProductionPath)) {
        $failedPath = $ProductionPath + '_failed_' + (Get-Date -Format 'yyyyMMdd_HHmmss')
        Move-Item -LiteralPath $ProductionPath -Destination $failedPath
        Write-Warning "La version en échec a été conservée dans $failedPath"
    }

    if ($script:BackupPath -and (Test-Path -LiteralPath $script:BackupPath)) {
        Move-Item -LiteralPath $script:BackupPath -Destination $ProductionPath
        Write-Warning "L'ancienne version a été restaurée dans $ProductionPath"
    }

    if ($script:ServiceCreated) {
        & sc.exe delete $ServiceName | Out-Null
    }
    elseif ($script:ExistingService -and $script:ServiceWasRunning) {
        Start-Service -Name $ServiceName -ErrorAction SilentlyContinue
    }

    foreach ($task in $script:LegacyTasksToReEnable) {
        Enable-ScheduledTask -InputObject $task | Out-Null
        Write-Warning "La tâche planifiée $($task.TaskName) a été réactivée."
    }
}

try {
    Write-Step 'Contrôles préalables'
    Assert-Administrator

    $resolvedStagingPath = [IO.Path]::GetFullPath($StagingPath)
    $resolvedProductionPath = [IO.Path]::GetFullPath($ProductionPath)
    if ($resolvedStagingPath -eq $resolvedProductionPath) {
        throw 'StagingPath et ProductionPath doivent être différents.'
    }

    Assert-StagingContent

    $script:LegacyTasks = @(Get-LegacyGenDbTask)
    if ($script:LegacyTasks.Count -gt 0) {
        foreach ($task in $script:LegacyTasks) {
            Write-Host "Tâche planifiée détectée : $($task.TaskPath)$($task.TaskName) ($($task.State))"
            if ($task.State -ne 'Disabled') {
                $script:LegacyTasksToReEnable += $task
            }
        }
    }
    else {
        Write-Warning "Aucune ancienne tâche planifiée GenDb n’a été trouvée."
    }

    $script:ExistingService = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
    if ($script:ExistingService) {
        $script:ServiceWasRunning = $script:ExistingService.Status -eq 'Running'
        Write-Host "Service existant détecté : $ServiceName ($($script:ExistingService.Status))"
    }

    Write-Step "Arrêt de l’ancien mécanisme"
    foreach ($task in $script:LegacyTasksToReEnable) {
        Disable-ScheduledTask -InputObject $task | Out-Null
        Write-Host "Tâche planifiée désactivée : $($task.TaskName)"
    }

    if ($script:ExistingService -and $script:ExistingService.Status -ne 'Stopped') {
        Stop-Service -Name $ServiceName -Force
        (Get-Service -Name $ServiceName).WaitForStatus('Stopped', [TimeSpan]::FromSeconds(30))
    }

    $runningGenDb = @(Get-Process -Name 'GenDb' -ErrorAction SilentlyContinue)
    if ($runningGenDb.Count -gt 0) {
        $processIds = ($runningGenDb.Id -join ', ')
        throw "Un processus GenDb est encore actif (PID : $processIds). Arrêtez-le puis relancez le script."
    }

    Write-Step 'Sauvegarde et activation de la nouvelle version'
    if (Test-Path -LiteralPath $ProductionPath) {
        $script:BackupPath = $ProductionPath + '_backup_' + (Get-Date -Format 'yyyyMMdd_HHmmss')
        Move-Item -LiteralPath $ProductionPath -Destination $script:BackupPath
        Write-Host "Ancienne version sauvegardée : $script:BackupPath"
    }

    Move-Item -LiteralPath $StagingPath -Destination $ProductionPath
    $script:DeploymentActivated = $true
    Write-Host "Nouvelle version activée : $ProductionPath"

    Write-Step 'Installation et configuration du service'
    $executablePath = Join-Path $ProductionPath 'GenDb.exe'
    if (-not $script:ExistingService) {
        New-Service `
            -Name $ServiceName `
            -BinaryPathName $executablePath `
            -DisplayName $ServiceDisplayName `
            -Description 'Valide et importe les exports GPAO dans la base Horizon.' `
            -StartupType Automatic | Out-Null
        $script:ServiceCreated = $true
    }
    else {
        Set-Service -Name $ServiceName -StartupType Automatic
        $quotedExecutablePath = '"' + $executablePath + '"'
        & sc.exe config $ServiceName 'binPath=' $quotedExecutablePath | Out-Null
        if ($LASTEXITCODE -ne 0) {
            throw "Impossible de mettre à jour le chemin du service $ServiceName."
        }
    }

    & sc.exe failure $ServiceName 'reset=' '86400' 'actions=' 'restart/60000/restart/60000/restart/60000' | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Impossible de configurer la récupération du service $ServiceName."
    }

    & sc.exe failureflag $ServiceName 1 | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Impossible d'activer la récupération du service $ServiceName."
    }

    Write-Step 'Démarrage et vérification'
    Start-Service -Name $ServiceName
    Wait-ServiceRunning -Name $ServiceName
    Start-Sleep -Seconds 5

    $service = Get-Service -Name $ServiceName
    if ($service.Status -ne 'Running') {
        throw "Le service $ServiceName s'est arrêté juste après son démarrage."
    }

    $logDirectory = Join-Path $ProductionPath 'logs'
    $latestLog = Get-ChildItem -LiteralPath $logDirectory -Filter 'GEN_DB_HORIZON_LOGS_*.log' -File -ErrorAction SilentlyContinue |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1
    if (-not $latestLog) {
        throw "Le service fonctionne mais aucun journal GenDb n'a été créé dans $logDirectory."
    }

    Write-Host "`nDéploiement réussi." -ForegroundColor Green
    Write-Host "Service : $ServiceName ($($service.Status))"
    Write-Host "Journal : $($latestLog.FullName)"
    if ($script:BackupPath) {
        Write-Host "Sauvegarde : $script:BackupPath"
    }
    Write-Host "`nDernières lignes du journal :" -ForegroundColor Cyan
    Get-Content -LiteralPath $latestLog.FullName -Tail 20
}
catch {
    $deploymentError = $_
    Write-Host "`nERREUR DE DÉPLOIEMENT : $($deploymentError.Exception.Message)" -ForegroundColor Red
    try {
        Restore-PreviousDeployment
    }
    catch {
        Write-Host "ERREUR DE RETOUR ARRIÈRE : $($_.Exception.Message)" -ForegroundColor Red
    }
    throw $deploymentError
}
