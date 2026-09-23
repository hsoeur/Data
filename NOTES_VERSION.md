# Notes de version

Date : 2026-06-08

## API Controllers

### Nommage des controllers au pluriel

Les controllers qui etaient encore declares au singulier ont ete renommes au pluriel afin d'harmoniser les routes exposees par l'API.

Controllers concernes :

- `DatabaseNotificationsController`
- `HealthsController`
- `KitTraceabilitiesController`
- `LabelTemplatesController`
- `LabelTypesController`
- `ManufacturingOrderNotificationsController`
- `ManufacturingOrdersController`
- `OperationsController`
- `OrderLinesController`
- `SequencesController`
- `SolderingProgramsController`

Les fichiers `HealthController.cs` et `SequenceController.cs` ont aussi ete renommes en `HealthsController.cs` et `SequencesController.cs`.

### Tolerance sur les anciens noms de routes

Des routes alias ont ete ajoutees pour conserver la compatibilite avec les anciens chemins singuliers.

Exemples :

- Nouveau chemin officiel : `api/ManufacturingOrders/...`
- Ancien chemin encore accepte : `api/ManufacturingOrder/...`
- Nouveau chemin officiel : `api/Sequences/...`
- Ancien chemin encore accepte : `api/Sequence/...`

Cette tolerance permet de migrer les clients progressivement vers les routes au pluriel.

## Measurement Traceabilities

### Controle de coherence ProductUid

Lors de la creation d'un enregistrement de mesure (`MeasurementTraceability` / MeasTraca), l'API verifie maintenant que le `ProductUid` de la `Tolerance` est identique au `ProductUid` de la `ProductTraceability`.

La regle appliquee est :

```text
Tolerance.ProductUid == ProductTraceability.ProductUid
```

Si la `Tolerance` est introuvable, si la `ProductTraceability` est introuvable, ou si les deux `ProductUid` sont differents, la creation est refusee.

### Endpoints concernes

Le controle est applique sur tous les modes de creation :

- Creation simple : `POST /api/MeasurementTraceabilities/create`
- Batch atomique : `POST /api/MeasurementTraceabilities/batch/atomic`
- Batch best-effort : `POST /api/MeasurementTraceabilities/batch`

### Comportement en cas d'erreur

- Creation simple : retourne `400 BadRequest`.
- Batch atomique : retourne `400 BadRequest` et aucun enregistrement du batch n'est cree.
- Batch best-effort : l'element incoherent est ajoute dans la liste des erreurs, les autres elements valides peuvent etre crees.

## Validation

Une compilation de verification a ete executee avec succes :

```powershell
dotnet build Api.Data\Api.Data.csproj -o .verify-build\Api.Data
```

Le dossier temporaire de verification `.verify-build` a ensuite ete supprime.
