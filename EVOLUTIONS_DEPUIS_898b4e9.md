# Evolutions depuis le commit `898b4e9`

## Perimetre

- Commit de reference exclu : `898b4e9d5f52639dcce13a85c397e538b7e44cdd`
- Date du commit de reference : 24 avril 2026 a 10:41:30 (+02:00)
- Libelle : `Implementation de la gestion des tolerance en production (Tolerance Manager)`
- Comparaison : `898b4e9..6af15ea`
- HEAD analyse : `6af15ea`
- Periode couverte : du 3 juin au 23 septembre 2026
- Nombre de commits : 16
- Ampleur cumulee : 78 fichiers, 5 466 insertions, 560 suppressions
- Etat du depot au moment de l'analyse : arbre de travail propre

Le commit `898b4e9` sert de point de depart et ses propres changements ne sont donc pas inclus.

## Synthese generale

Depuis `898b4e9`, les evolutions portent sur huit domaines :

1. suppression puis reconstruction complete de la tracabilite des mesures ;
2. normalisation des unites de tolerance ;
3. modification des modes de tolerance et ajout d'une validation stricte ;
4. changements de routes, noms de controleurs, parametres et formes de reponse ;
5. ajout de fonctions metier : commentaire de panne, copie de tolerances, validation d'OF, creation d'etiquettes par lot ;
6. fiabilisation de certains imports GenDb ;
7. apparition d'un projet de tests unitaires ;
8. ajout d'elements Docker, de documentation et d'artefacts binaires.

## Chronologie exhaustive des commits

### 1. `6d35128` - 3 juin 2026

**Suppression de MeasTracea pour repartir sur une base saine avant la version 2.1.0**

- Suppression du `DbSet<MeasurementTraceability>` de `AppDbContext`.
- Ajout de la migration `20260603075926_RemoveMeasTracea`.
- La methode `Up` de cette migration execute un `DROP TABLE MeasurementTraceabilities`.
- La methode `Down` peut recreer l'ancien schema de mesure avec :
  - `ProductUid` ;
  - `SerialNumber` ;
  - `Label` ;
  - `Value` ;
  - `Classification` ;
  - `CreationDate` ;
  - `OperationNumber` ;
  - `UnitUid` ;
  - `UserUid` ;
  - `MachineUid` ;
  - `ManufacturingOrderUid` nullable ;
  - les anciennes cles etrangeres et les index associes.
- Ajout du script SQL `Migrations/sql/removeTraceaMeas.sql`, qui supprime egalement la table et inscrit la migration dans `__efmigrationshistory`.
- Mise a jour du snapshot EF.
- Modification documentaire uniquement de l'exemple `EquipSN` dans `DeviceIdentity` : `00634A` devient `0634A`.

Impact majeur : la migration montante supprime toutes les mesures existantes.

### 2. `d5fe4b5` - 3 juin 2026

**Creation de la table des unites et retrait temporaire de toute la couche MeasurementTraceability**

- Suppression du controleur `MeasurementTraceabilitiesController`.
- Suppression du modele `MeasurementTraceability`.
- Suppression de l'interface et de l'implementation du repository des mesures.
- Suppression des DTO specialises pour la creation de mesure avec adresse MAC :
  - `MeasurementTraceabilityWithMacDto` ;
  - `MeasurementTraceabilityWithMacResponseDto`.
- Retrait de l'enregistrement du repository de mesures dans l'injection de dependances.
- Transformation de `Unit` :
  - auparavant classe statique de constantes (`Unknown`, `Volt`, `Ampere`, `Ohm`, `Celsius`, `Percent`, `Second`, `Hertz`) ;
  - devient une entite EF avec `UnitUid` comme cle primaire et `Label`.
- Ajout de `DbSet<Unit> Units`.
- Transformation du modele et du DTO `Tolerance` : la propriete texte `Unit` devient la cle etrangere `UnitUid` vers `Units`.
- Adaptation des operations de creation, mise a jour et historisation des tolerances a `UnitUid`.
- Ajout de la migration `20260603102622_UnitAndDeleteMeas` :
  - supprime immediatement `Tolerances.Unit` ;
  - ajoute `Tolerances.UnitUid` non nullable avec valeur par defaut vide ;
  - cree `Units` ;
  - cree l'index sur `Tolerances.UnitUid` ;
  - cree la cle etrangere restrictive vers `Units`.
- Ajout du script SQL `units.sql` equivalent.
- Mise a jour du snapshot EF.
- Ajout temporaire d'une declaration de dossier SQL dans `Migrations.csproj`.

Impact de contrat : les clients envoyant ou lisant `Unit` doivent maintenant utiliser `UnitUid`.

### 3. `f3fe33c` - 3 juin 2026

**Alignement des noms de parametres de KitTraceability**

- `GET api/KitTraceability/is-already-processed` : le parametre query `uidOf` devient `ManufacturingOrderUid`.
- `GET api/KitTraceability/by-product-serial` :
  - `productUid` devient `Product` ;
  - `serialNumber` devient `SerialNumber`.
- Ajout de validations : produit obligatoire et numero de serie strictement positif.
- Renommage des methodes et parametres correspondants dans l'interface et le repository.
- La recherche continue fonctionnellement a filtrer `KitTraceabilities.Product` et `SerialNumber`.
- Mise a jour des messages Swagger, des logs et des messages d'erreur.

Impact de contrat : les anciens noms de query string ne sont plus acceptes explicitement.

### 4. `29cc4dd` - 3 juin 2026

**Ajustement de la selection de la derniere trace produit**

- Dans le repository de tracabilite produit, `FirstOrDefault()` devient `First()` apres regroupement par OF et numero de serie.
- Chaque groupe provenant necessairement de donnees existantes, le resultat ne contient plus d'element nullable a ce niveau.
- Ce commit seul ne change pas encore le code HTTP ; le changement HTTP est finalise dans le commit suivant.

### 5. `7c2e6ca` - 3 juin 2026

**Correction du contrat de l'endpoint de derniere tracabilite par OF/operation**

- Endpoint concerne : `GET .../latest-by-manufacturing-order-and-operation`.
- Le type documente et retourne passe d'un objet `ProductTraceability` a une collection `ICollection<ProductTraceability>`.
- La description precise que la reponse contient les dernieres traces regroupees par numero de serie.
- En absence de resultat : passage de `404 NotFound` a `204 NoContent`.
- La declaration Swagger est alignee sur `200 collection` et `204`.

Impact de contrat : un client qui deserialise un objet unique doit maintenant deserialiser un tableau.

### 6. `4022e52` - 3 juin 2026

**Refonte et validation des modes de tolerance**

- Anciennes valeurs supprimees :
  - `Unknown` ;
  - `NotEqual` ;
  - `Less` ;
  - `LessOrEqual` ;
  - `Greater` ;
  - `GreaterOrEqual` ;
  - `BetweenInclusive` ;
  - `BetweenExclusive`.
- Nouvelles valeurs autorisees :
  - `Between` ;
  - `Equal` ;
  - `Min` ;
  - `Max` ;
  - `Record` ;
  - `Entry`.
- Ajout de `ValidModeAttribute`, qui enumere par reflexion les constantes publiques de `Mode`.
- Application de cet attribut a `Tolerance.Mode` et `ToleranceDto.Mode`.
- Les actions de creation et de mise a jour des tolerances verifient `ModelState` et retournent `400` si le mode est absent ou inconnu.

Impact donnees/contrat : les valeurs historiques retirees ne satisfont plus la nouvelle validation.

### 7. `49b6c1f` - 3 juin 2026

**Reformatage de `ValidModeAttribute`**

- Aucun changement fonctionnel.
- Reindentation et mise en forme du code introduit dans le commit precedent.

### 8. `4f6888f` - 3 juin 2026

**Reconstruction de MeasurementTraceability dans un nouveau format**

- Recreation du modele `MeasurementTraceability` avec seulement :
  - `MeasurementTraceabilityUid` ;
  - `ProductTraceabilityUid` et navigation vers `ProductTraceability` ;
  - `ToleranceUid` et navigation vers `Tolerance` ;
  - `Value`, limite a 128 caracteres ;
  - `Classification`, limitee a 32 caracteres.
- Les anciennes colonnes directes ne font plus partie de la mesure : produit, numero de serie, libelle, date, operation, unite, utilisateur, machine et OF doivent etre retrouves via les relations.
- Nouveau DTO de creation : `ProductTraceabilityUid`, `ToleranceUid`, `Value`, `Classification`.
- Ajout de DTO de resultat pour les lots best-effort avec `Created`, `Errors`, index de l'element et message.
- Recreation du controleur de mesures avec quatre endpoints :
  - `POST /api/MeasurementTraceabilities/create` ;
  - `POST /api/MeasurementTraceabilities/batch/atomic` ;
  - `POST /api/MeasurementTraceabilities/batch` ;
  - `GET /api/MeasurementTraceabilities/by-product-and-operation`.
- Le batch atomique utilise une transaction et la strategie de reexecution EF : tout le lot est valide ou annule.
- Le batch best-effort sauvegarde element par element, conserve les succes et retourne les erreurs avec leur index d'origine.
- Validation des champs obligatoires et des longueurs maximales.
- Generation serveur de l'identifiant de mesure par GUID.
- Recreation de l'interface et du repository de mesures ; reenregistrement dans l'injection de dependances.
- Ajout de trois index composes sur les mesures :
  - `(ProductTraceabilityUid, ToleranceUid)` ;
  - `(ToleranceUid, Classification)` ;
  - `(ProductTraceabilityUid, Classification)`.
- Ajout de la migration `20260603133843_AddMeasurementTraceability`, qui recree la table sous son nom historique mais avec le nouveau schema et deux cles etrangeres restrictives.
- Ajout de l'index MySQL `IX_ProductTraceabilities_ProductUid_OperationNumber` avec prefixe `ProductUid(255)`.
- Ajout des scripts SQL `meas.sql` et `meas2.sql`.
- Mise a jour du snapshot EF.
- Retrait de la declaration de dossier ajoutee precedemment dans `Migrations.csproj`.

Impact majeur : il ne s'agit pas d'une evolution additive de la table, mais d'une suppression/recreation avec un contrat entierement different.

### 9. `67ac0cb` - 4 juin 2026

**Ajout du commentaire de panne aux traces produit**

- Ajout de `FailureComment`, chaine nullable, dans `ProductTraceability`.
- Ajout du meme champ dans `ProductTraceabilityDto`.
- L'action de creation copie le commentaire du DTO vers l'entite.
- Ajout de la colonne nullable `ProductTraceabilities.FailureComment` en `longtext`.
- Ajout de la migration `20260604110000_AddProductTraceabilityFailureComment` et du script `failureComment.sql`.
- Mise a jour du snapshot EF.

Evolution additive et retrocompatible au niveau du schema.

### 10. `a2c3d60` - 8 juin 2026

**Pluralisation de onze controleurs**

Renommages :

- `DatabaseNotificationController` vers `DatabaseNotificationsController` ;
- `HealthController` vers `HealthsController` ;
- `KitTraceabilityController` vers `KitTraceabilitiesController` ;
- `LabelTemplateController` vers `LabelTemplatesController` ;
- `LabelTypeController` vers `LabelTypesController` ;
- `ManufacturingOrderNotificationController` vers `ManufacturingOrderNotificationsController` ;
- `ManufacturingOrderController` vers `ManufacturingOrdersController` ;
- `OperationController` vers `OperationsController` ;
- `OrderLineController` vers `OrderLinesController` ;
- `SequenceController` vers `SequencesController` ;
- `SolderingProgramController` vers `SolderingProgramsController`.

Les routes officielles basees sur `[controller]` deviennent donc plurielles. A ce stade, le commit ajoute aussi des alias singuliers pour conserver la compatibilite : `api/Health`, `api/KitTraceability`, `api/ManufacturingOrder`, etc.

Les fichiers physiques `HealthController.cs` et `SequenceController.cs` sont renommes.

### 11. `e5f176d` - 8 juin 2026

**Controle de coherence entre une mesure, sa tolerance et sa trace produit**

- Avant chaque creation de mesure, le repository charge le `ProductUid` de la `ProductTraceability` et celui de la `Tolerance`.
- La creation est refusee si la trace produit n'existe pas.
- La creation est refusee si la tolerance n'existe pas.
- La creation est refusee si les deux `ProductUid` sont differents.
- Le controle s'applique a la creation simple, au batch atomique et a chaque element du batch best-effort.
- Les `ArgumentException` deviennent des reponses `400 BadRequest` pour la creation simple et atomique.
- Dans le batch best-effort, l'erreur est rattachee a l'element concerne et les autres elements peuvent etre crees.

### 12. `24536ee` - 10 juin 2026

**Uniformisation des routes par attributs explicites, avec retrait des alias singuliers**

- `DatabaseNotifications` passe de `api/[controller]/[action]` a `api/[controller]` avec chemins d'action explicites :
  - `GetAll` ;
  - `Create` ;
  - `Search` ;
  - `Process`.
- `Healths` suit la meme logique avec :
  - `Live` ;
  - `Database` ;
  - `Config` ;
  - `Get`.
- Suppression de tous les alias singuliers ajoutes par `a2c3d60` pour :
  - `DatabaseNotification` ;
  - `Health` ;
  - `KitTraceability` ;
  - `LabelTemplate` ;
  - `LabelType` ;
  - `ManufacturingOrderNotification` ;
  - `ManufacturingOrder` ;
  - `Operation` ;
  - `OrderLine` ;
  - `Sequence` ;
  - `SolderingProgram`.
- Ajout de plusieurs artefacts locaux a `.gitignore` : archives Docker/API, exports SQL/CSV et repertoire de test.

Impact de contrat : a partir de ce commit, seules les routes plurielles demeurent pour ces controleurs. Cela contredit les notes de version ajoutees ulterieurement, qui affirment que les alias sont encore disponibles.

### 13. `aa16c82` - 18 juin 2026

**Fiabilisation du generateur d'articles**

- `ArticleGenerator.Update()` effectue maintenant jusqu'a trois tentatives.
- Attente de cinq secondes entre deux tentatives en echec.
- Journalisation distincte des tentatives intermediaires et de l'echec final.
- Lecture de `GpaoQuantity` via `CsvParser.TryParseInt` plutot que `int.TryParse` direct.
- Journalisation d'un `ParseFail` lorsque la quantite ne peut pas etre convertie.

### 14. `0676156` - 19 aout 2026

**Nouveau gestionnaire de copie de tolerances en production**

- Ajout de `POST /api/Tolerances/batch-copy`.
- La requete contient : produit source, produit cible, liste de tolerances, date de debut, date de fin et utilisateur.
- La reponse contient : source, cible, nombre cree et DTO des tolerances creees.
- Ajout de `ToleranceUid` nullable au `ToleranceDto` de sortie.
- Validations controleur :
  - source obligatoire ;
  - cible obligatoire ;
  - au moins une tolerance ;
  - utilisateur obligatoire ;
  - date de fin posterieure a la date de debut.
- Copie executee dans une transaction avec strategie de reexecution EF.
- Deduplication stricte des UIDs demandes ; un UID vide ou duplique invalide tout le lot.
- Verification de l'existence du produit source.
- Refus si le produit cible possede deja au moins une tolerance.
- Verification que toutes les tolerances selectionnees existent et appartiennent au produit source.
- Creation de nouveaux GUID pour les tolerances cible.
- Copie des operation, libelle, machine, unite, valeurs et mode ; remplacement du produit, de l'utilisateur et des dates par les valeurs de la requete.
- Ajout de trois exceptions metier et de leurs correspondances HTTP :
  - source introuvable : `404` ;
  - cible possedant deja des tolerances : `409` ;
  - selection invalide : `400`.
- Reponse de succes `201 Created`.
- Corrections d'encodage de plusieurs textes francais dans le controleur et le repository.

### 15. `d888f18` - 23 septembre 2026

**Validation d'un ordre de fabrication**

- Ajout de `POST /api/ManufacturingOrders/validate`.
- Ajout d'une requete fortement validee :
  - suffixe produit de exactement 6 caracteres, espace final significatif ;
  - numero de serie de exactement 6 chiffres ;
  - identifiant OF de exactement 5 chiffres.
- Chargement de l'OF par identifiant via une nouvelle methode repository `AsNoTracking`.
- Verification que `ProductUid` se termine par le suffixe, sans `Trim`, comparaison insensible a la casse.
- Calcul inclusif de la plage de numeros de serie : `FirstSerialNumber` a `FirstSerialNumber + Quantity - 1`.
- Protection contre quantite non positive et depassement de `int`.
- Reponse structuree avec : validite globale, correspondance produit, appartenance a la plage, OF, produit, premier et dernier numeros et message.
- Reponses : `200` pour le resultat metier, `400` pour les formats invalides via `[ApiController]`, `404` si l'OF n'existe pas, `500` masque par un `ProblemDetails` generique.
- Creation du projet de tests `Api.Data.Tests` sous .NET 9 avec xUnit.
- Ajout de tests sur : bornes inclusives, suffixe avec espace, non-correspondance produit, OF absent et formats exacts.

### 16. `6af15ea` - 23 septembre 2026

**Creation d'etiquettes par lot, corrections GenDb, environnement Docker et documentation**

#### Etiquettes

- Ajout de `POST /api/Labels/create-batch`.
- Requete : collection de `LabelDto`.
- Refus d'un corps null ou vide avec `400`.
- Refus d'un `AuthentiUid` duplique dans le lot avec `400`.
- Generation d'un GUID `LabelUid` pour chaque etiquette.
- Mapping de tous les champs du DTO vers `Label`.
- Insertion de tout le lot avec `AddRangeAsync` et un seul `SaveChangesAsync`.
- Timeout de commande positionne a 600 secondes avant la sauvegarde.
- Reponse `200` avec `createdCount`.
- Ajout de tests pour lot vide, doublons et mapping/persistance du lot.

#### Tests

- Correction d'une coupure de ligne erronee dans `new ValidationContext(request)` dans les tests d'OF.
- Ajout du projet de tests a `DataProd.sln` pour toutes les configurations Debug/Release et plateformes.

#### GenDb

- `CsvParser` resout maintenant le chemin complet du fichier et journalise chemin, date de modification et taille avant lecture.
- `ProductFirmwareGenerator` change sa cle logique :
  - avant : une version par couple `(ArticleUid, Version)` ;
  - apres : une seule ligne conservee par `ArticleUid`.
- Pour un article existant, la version est mise a jour si necessaire.
- Les versions firmware dupliquees pour le meme article sont supprimees.
- Le resume journalise maintenant insertions, mises a jour, suppressions, lignes ignorees.

#### Configuration locale et Docker

- Valeurs de connexion par defaut de l'API modifiees de `mysql-prod:3306` vers `localhost:13306`.
- Profils de lancement modifies : HTTP principal sur `10000`, second profil HTTP sur `5400`.
- Ajout d'un `Dockerfile` multi-stage .NET 9 exposant le port 10000.
- Ajout d'un `docker-compose.yml` avec :
  - MySQL 8.0.36 sur le port hote 13306 ;
  - volume persistant `mysql-data` ;
  - import optionnel de `db_production.sql` ;
  - health check MySQL ;
  - phpMyAdmin 5.2.1 sur le port 9000 ;
  - API `api_data_211` sur le port 10000 ;
  - configuration MySQL et fuseau Europe/Paris par variables d'environnement.
- Ajout d'un guide Docker de construction, execution, export/import et depannage.
- Ajout au depot de deux artefacts binaires lourds :
  - `api_data_211.tar`, 97 250 816 octets ;
  - `api_data_211.zip`, 96 769 528 octets.

#### Documentation

- Ajout de `NOTES_VERSION.md` sur la pluralisation, les alias et la coherence des mesures.
- Ajout de `PLAN_MIGRATION_PRODUCTION.md` avec la strategie Expand/Migrate/Contract, les risques, le blue/green et les criteres Go/No-Go.
- Incoherence documentaire : `NOTES_VERSION.md` dit que les alias singuliers sont conserves, mais ils ont ete supprimes par `24536ee` et ne sont pas presents au HEAD.

#### Changements sans effet fonctionnel notable

- Ligne vide dans `ArticlesController`.
- Virgule finale dans un `ProblemDetails` de `MacAddressesController`.
- Deux lignes vides dans `TolerancesController`.

## Etat final du contrat API par rapport a `898b4e9`

### Endpoints ajoutes

- `POST /api/MeasurementTraceabilities/batch/atomic`
- `POST /api/MeasurementTraceabilities/batch`
- `GET /api/MeasurementTraceabilities/by-product-and-operation`
- `POST /api/Tolerances/batch-copy`
- `POST /api/ManufacturingOrders/validate`
- `POST /api/Labels/create-batch`

### Endpoints ou contrats retires/remplaces

- Ancienne representation de `POST /api/MeasurementTraceability/create` remplacee par le nouveau DTO de mesure.
- `create-with-mac` retire avec ses DTO specialises.
- `by-article-and-operation` remplace par `by-product-and-operation`.
- Routes singulieres retirees au profit des noms pluriels pour onze controleurs.
- Parametres KitTraceability renommes.
- Reponse objet de `latest-by-manufacturing-order-and-operation` remplacee par une collection ; absence de resultat en `204` au lieu de `404`.

## Etat final du schema par rapport a `898b4e9`

- Table `MeasurementTraceabilities` supprimee puis recreee avec un schema incompatible.
- Anciennes mesures non migrees par les migrations fournies.
- Nouvelle table `Units`.
- `Tolerances.Unit` remplace par `Tolerances.UnitUid` non nullable et lie a `Units`.
- `Tolerance.Mode` limite a six valeurs.
- `ProductTraceabilities.FailureComment` ajoute et nullable.
- Nouveaux index de mesures et index de recherche produit/operation.

## Points de vigilance derives de l'historique

1. `DROP TABLE MeasurementTraceabilities` provoque une perte de donnees si la migration est appliquee sur une base contenant l'historique.
2. `Tolerances.Unit` est supprime sans backfill explicite vers `UnitUid`.
3. `UnitUid` est ajoute avec `''`, puis une cle etrangere est creee ; l'ordre de peuplement de `Units` est donc critique.
4. Les anciennes valeurs de `Mode` ne sont pas converties vers les nouvelles.
5. Les alias singuliers ont existe entre `a2c3d60` et `24536ee`, puis ont ete retires.
6. Le changement d'un objet vers une collection et les renommages de query string sont incompatibles pour les consommateurs existants.
7. La logique firmware supprime des doublons et change la cardinalite metier a une version par article ; cette suppression doit etre validee fonctionnellement.
8. Les archives Docker ajoutees representent environ 194 Mo dans Git et dupliquent potentiellement un artefact qui devrait etre gere par un registre ou le pipeline.

