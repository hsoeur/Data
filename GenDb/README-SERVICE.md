# Service d'import GenDb

GenDb fonctionne désormais comme un service Windows qui surveille les sept exports GPAO.

## Conditions de déclenchement

Un lot est importé uniquement lorsque :

- les sept fichiers configurés dans `CsvPaths` existent et ne sont pas vides ;
- chacun a changé depuis le dernier import réussi ;
- leurs horodatages tiennent dans la fenêtre `MaximumTimestampSpreadMinutes` ;
- aucun fichier n'est plus ancien que `MaximumFileAgeHours` ;
- leurs tailles et dates restent identiques pendant `StabilitySeconds` ;
- chaque fichier peut être ouvert exclusivement ;
- les CSV sont lisibles, ont le nombre de colonnes attendu et leurs identifiants obligatoires sont valides ;
- le volume d'un fichier ne chute pas sous `MinimumRowCountRatio` du dernier lot.

Le service copie ensuite le lot dans `ProcessingRoot`, compare les SHA-256 des sources et des copies, puis importe uniquement le snapshot. Tous les générateurs partagent une transaction : un échec annule le lot complet.

L'état du dernier succès est conservé dans `StateFile`. Ne supprimez ce fichier que si vous souhaitez autoriser explicitement le retraitement du lot courant.

## Publication

Depuis la racine du dépôt :

```powershell
dotnet publish .\GenDb\GenDb.csproj -c Release -r win-x64 --self-contained false -o C:\BackServices\GenDb2.0
```

Arrêtez au préalable l'ancienne tâche planifiée pour éviter deux imports concurrents.

## Installation du service

La procédure recommandée consiste à utiliser le script de déploiement après avoir publié dans `C:\BackServices\GenDb2.0_next` :

```powershell
.\GenDb\Deploy-GenDbService.ps1
```

Si plusieurs tâches planifiées contiennent `GenDb`, indiquez explicitement l'ancienne tâche :

```powershell
.\GenDb\Deploy-GenDbService.ps1 `
    -LegacyTaskName 'Nom exact de la tâche' `
    -LegacyTaskPath '\'
```

Le script contrôle la publication et les CSV, désactive l'ancienne tâche, sauvegarde le dossier de production, installe ou met à jour le service, configure sa récupération et restaure automatiquement l'ancienne version en cas d'échec.

### Installation manuelle

Dans un terminal PowerShell administrateur :

```powershell
New-Service `
    -Name 'GenDbGPAO' `
    -BinaryPathName 'C:\BackServices\GenDb2.0\GenDb.exe' `
    -DisplayName 'GenDb - Import GPAO' `
    -Description 'Valide et importe les exports GPAO dans la base Horizon.' `
    -StartupType Automatic

Start-Service -Name 'GenDbGPAO'
```

Le compte du service doit avoir les droits de lecture sur `C:\Data\Export_GPAO`, les droits de modification sur `C:\Data\GenDb` et l'accès à MySQL.

## Exploitation

```powershell
Get-Service -Name 'GenDbGPAO'
Restart-Service -Name 'GenDbGPAO'
Stop-Service -Name 'GenDbGPAO'
```

Les journaux sont écrits quotidiennement dans le dossier `logs` placé à côté de l'exécutable. Un snapshot ayant échoué pendant l'import est conservé pour diagnostic ; un snapshot importé est supprimé, sauf si `KeepSuccessfulSnapshots` vaut `true`.
