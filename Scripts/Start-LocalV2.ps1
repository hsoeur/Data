[CmdletBinding()]
param(
    [switch]$NoBuild
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$composeFile = Join-Path $repositoryRoot 'docker-compose.local-v2.yml'
$environmentFile = Join-Path $repositoryRoot '.env.local-v2'
$importDirectory = Join-Path $repositoryRoot 'local-v2\import'

$dumpFiles = Get-ChildItem -LiteralPath $importDirectory -File |
    Where-Object { $_.Name -match '\.sql(?:\.gz|\.xz|\.zst)?$' }

if (-not (Test-Path -LiteralPath $environmentFile)) {
    throw "Missing $environmentFile. Copy .env.local-v2.example and set local passwords."
}

if ($dumpFiles.Count -eq 0) {
    throw "No production V2 dump found in $importDirectory. Expected a .sql or compressed SQL file."
}

Push-Location $repositoryRoot
try {
    & docker compose --env-file $environmentFile -f $composeFile config --quiet
    if ($LASTEXITCODE -ne 0) {
        throw 'The local V2 Docker Compose configuration is invalid.'
    }

    $arguments = @(
        'compose',
        '--env-file', $environmentFile,
        '-f', $composeFile,
        'up', '-d',
        '--wait',
        '--wait-timeout', '600'
    )

    if (-not $NoBuild) {
        $arguments += '--build'
    }

    & docker @arguments
    if ($LASTEXITCODE -ne 0) {
        throw 'The local V2 stack failed to start or become healthy.'
    }

    Write-Host ''
    Write-Host 'Local V2 environment is ready:'
    Write-Host '  API:        http://127.0.0.1:11000'
    Write-Host '  Swagger:    http://127.0.0.1:11000/swagger'
    Write-Host '  phpMyAdmin: http://127.0.0.1:9081'
    Write-Host '  MySQL:      127.0.0.1:23306'
}
finally {
    Pop-Location
}
