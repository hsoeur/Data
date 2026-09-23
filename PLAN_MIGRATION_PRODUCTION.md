# Plan de migration API Data et base de donnees

## 1. Objectif

Faire evoluer l'API Data et le modele de donnees sans interrompre les logiciels de production, sans perdre les mesures historiques et avec un retour arriere rapide de l'application.

La strategie recommandee est **Expand / Migrate / Contract** :

1. ajouter le nouveau schema sans supprimer l'ancien ;
2. rendre l'API compatible avec les anciens et les nouveaux consommateurs ;
3. migrer les donnees et les logiciels progressivement ;
4. observer une periode de stabilite ;
5. supprimer les anciens contrats et anciennes colonnes seulement dans une version ulterieure.

## 2. Perimetre analyse

Analyse statique du depot courant, compare a `origin/master`.

Elements observes :

- API ASP.NET Core 9 ;
- MySQL 8.0.36 ;
- migrations Entity Framework Core 9 ;
- scripts SQL manuels ;
- image de reference `api_data_211` ;
- pipeline Azure DevOps de deploiement Docker.

Limite : les depots et configurations des logiciels consommateurs ne sont pas presents ici. Leur impact individuel devra etre complete dans la matrice d'inventaire de la section 5.

## 3. Constats et risques identifies

### 3.1 Risques critiques

| Changement | Risque | Decision recommandee |
|---|---|---|
| La migration `20260603075926_RemoveMeasTracea` supprime `MeasurementTraceabilities` | Perte definitive des mesures historiques | Ne pas appliquer en production ; la remplacer par une migration additive |
| La nouvelle table `MeasurementTraceabilities` n'a plus la meme structure | Ancienne et nouvelle API ne peuvent pas utiliser la meme table | Creer `MeasurementTraceabilitiesV2` ou des tables explicitement versionnees |
| `Tolerances.Unit` est supprime avant ajout/remplissage de `UnitUid` | Perte de l'unite existante et violation possible de cle etrangere | Ajouter `UnitUid` nullable, remplir `Units`, backfiller, valider, puis rendre obligatoire plus tard |
| Plusieurs routes singulieres sont remplacees par des routes plurielles | Reponses 404 pour les logiciels existants | Conserver les alias singuliers pendant toute la transition |
| Le commit `24536ee` retire les alias de compatibilite alors que `NOTES_VERSION.md` indique qu'ils sont conserves | Documentation et comportement divergent | Retablir les alias et ajouter des tests de contrat |
| Le pipeline arrete le conteneur, le supprime et supprime son image avant validation de la nouvelle version | Interruption et rollback lent ou impossible | Passer a un deploiement blue/green et conserver au moins l'image N-1 |

### 3.2 Changements de contrat API incompatibles

1. Les controleurs suivants passent d'un nom singulier a un nom pluriel :
   `DatabaseNotification`, `Health`, `KitTraceability`, `LabelTemplate`, `LabelType`, `ManufacturingOrderNotification`, `ManufacturingOrder`, `Operation`, `OrderLine`, `Sequence`, `SolderingProgram`.

2. Le DTO de mesure change completement :

   - ancien contrat : `ProductUid`, `SerialNumber`, `Label`, `Value`, `Classification`, `CreationDate`, `OperationNumber`, `UnitUid`, `UserUid`, `MachineUid`, `ManufacturingOrderUid` ;
   - nouveau contrat : `ProductTraceabilityUid`, `ToleranceUid`, `Value`, `Classification`.

3. Les endpoints de mesure changent :

   - suppression de `create-with-mac` ;
   - remplacement de `by-article-and-operation` par `by-product-and-operation` ;
   - ajout de `batch` et `batch/atomic` ;
   - nouvelle validation de coherence entre le produit de la tolerance et celui de la trace produit.

4. `KitTraceabilities` renomme des parametres de requete :

   - `uidOf` devient `ManufacturingOrderUid` ;
   - `productUid` devient `Product` ;
   - `serialNumber` devient `SerialNumber`.

5. Un endpoint de `ProductTracabilities` annonce maintenant une collection la ou l'ancien contrat annoncait un objet unique. Les clients qui deserialisent un objet ne pourront pas lire un tableau.

6. `Tolerance.Unit` devient `Tolerance.UnitUid`, avec une table `Units`, et `Mode` est maintenant valide contre une liste fermee (`Between`, `Equal`, `Min`, `Max`, `Record`, `Entry`).

### 3.3 Changements additifs a faible risque

- `ProductTraceabilities.FailureComment` est nullable ;
- les nouveaux index de recherche sont additifs, sous reserve de mesurer leur duree de creation et leur impact de verrouillage sur une copie representant le volume de production ;
- les endpoints batch sont additifs s'ils ne remplacent pas les anciens endpoints.

### 3.4 Risques d'exploitation

- Les scripts SQL utilisent `START TRANSACTION` autour de DDL MySQL. Il ne faut pas considerer cela comme un mecanisme de rollback fiable.
- L'API ne lance pas automatiquement les migrations au demarrage. Le schema et l'application peuvent donc etre deployes dans le mauvais ordre si la procedure n'est pas explicite.
- Le pipeline actuel n'effectue ni health check apres demarrage, ni smoke test, ni bascule conditionnelle.
- Un secret de deploiement apparait directement dans le YAML du pipeline. Il doit etre revoque, remplace et stocke dans un secret Azure DevOps avant le prochain deploiement.
- Les changements locaux non valides du depot doivent etre separes du lot de migration avant de construire l'artefact de production.

## 4. Architecture de transition recommandee

### 4.1 Versionnement API

Conserver deux contrats pendant la migration :

- `/api/v1/...` : contrat de production actuel, sans changement de forme ;
- `/api/v2/...` : nouveaux DTO, nouvelles routes et nouvelles regles.

Si l'ajout immediat de `/v1` aux clients existants est impossible, les routes actuelles restent les alias v1, et seules les nouvelles routes sont publiees sous `/api/v2`.

Les alias singuliers et pluriels doivent appeler la meme logique metier. Leur usage doit etre journalise afin de savoir quels logiciels utilisent encore l'ancien contrat.

### 4.2 Schema temporaire compatible

Etape d'expansion uniquement :

1. creer `Units` et inserer les valeurs de reference ;
2. ajouter `Tolerances.UnitUid` nullable sans supprimer `Tolerances.Unit` ;
3. backfiller `UnitUid` avec une table de correspondance validee par le metier ;
4. conserver l'ancienne table `MeasurementTraceabilities` ;
5. creer `MeasurementTraceabilitiesV2` avec `ProductTraceabilityUid` et `ToleranceUid` ;
6. ajouter `FailureComment` et les index ;
7. ne rendre `UnitUid` obligatoire qu'apres controle de 100 % des lignes.

Pendant la transition, l'API v1 ecrit dans l'ancien format. L'API v2 ecrit dans V2. Si les memes mesures doivent etre visibles des deux cotes, mettre en place un double-write applicatif idempotent ou un adaptateur de lecture. Eviter les triggers caches, plus difficiles a observer et a rejouer.

## 5. Inventaire obligatoire des consommateurs

Creer une ligne par logiciel, service, script, poste ou outil BI qui appelle l'API ou lit directement la base.

| Consommateur | Responsable | Environnement | URL/routes utilisees | DTO lu/ecrit | Acces direct DB | Criticite | Fenetre d'arret admise | Version compatible v2 | Test signe | Migre |
|---|---|---|---|---|---|---|---|---|---|---|
| A completer | | | | | | | | | | |

Sources d'inventaire :

- recherche dans les depots (`api/`, noms des controleurs et port 10000) ;
- journaux d'acces API sur au moins un cycle complet de production ;
- configurations des postes et services Windows ;
- taches planifiees, scripts, outils de reporting et connexions SQL ;
- entretiens avec les responsables des lignes de production.

Un consommateur non identifie doit etre traite comme encore dependant de v1.

## 6. Migration des donnees de mesure

Le passage de l'ancien format au nouveau n'est pas une simple copie. Il faut retrouver deux references : `ProductTraceabilityUid` et `ToleranceUid`.

### 6.1 Regles de rapprochement a valider

Pour chaque ancienne mesure :

1. rechercher la trace produit avec le produit, le numero de serie, l'OF et l'operation ;
2. si plusieurs traces existent, utiliser la machine et la date la plus proche selon une tolerance temporelle approuvee ;
3. rechercher la tolerance avec le produit, l'operation, la machine, le libelle et sa periode de validite ;
4. convertir l'unite historique vers un `UnitUid` avec une table de correspondance explicite ;
5. ne migrer automatiquement que les correspondances uniques ;
6. placer les lignes sans correspondance ou ambiguës dans une table de rejet avec le motif ;
7. faire arbitrer les rejets par le metier, sans inventer de cle etrangere.

### 6.2 Proprietes du programme de migration

- idempotent : relancable sans doublon ;
- traitement par lots avec checkpoint ;
- journal du nombre lu, migre, ignore et rejete ;
- cle de correlation entre la ligne v1 et la ligne v2 ;
- controles avant/apres par produit, OF, operation, jour et classification ;
- aucune suppression de la source durant la migration ;
- essai chronometre sur une restauration anonymisee de production.

## 7. Plan de deploiement

### Phase 0 - Preparation et gel

1. designer un responsable technique, un responsable metier et un decideur Go/No-Go ;
2. fixer RTO, RPO, periode d'observation et fenetre de changement ;
3. geler les changements non lies a la migration ;
4. etablir l'inventaire complet des consommateurs ;
5. exporter les specifications OpenAPI v1 et v2 et produire un diff de contrat ;
6. sauvegarder la base, puis prouver la restauration sur un environnement isole ;
7. revoquer le secret expose dans le pipeline et le remplacer par une variable secrete.

Livrable de sortie : inventaire signe et aucun consommateur inconnu critique.

### Phase 1 - Release de compatibilite

1. retablir les routes alias singulieres et les anciens noms de parametres ;
2. maintenir les anciennes formes de reponse ;
3. publier v2 separement ;
4. ajouter des tests automatiques de contrat v1 et v2 ;
5. journaliser la version/route, le consommateur, le code HTTP, la latence et les erreurs de deserialisation connues.

Livrable de sortie : tous les tests de non-regression v1 passent avant toute migration de schema.

### Phase 2 - Expansion du schema

1. restaurer une copie recente de production en preproduction ;
2. executer les controles de qualite des donnees ;
3. appliquer uniquement les migrations additives ;
4. verifier les temps de verrouillage et la taille des index ;
5. executer les tests v1 avec l'ancien binaire API ;
6. executer les tests v1 et v2 avec le nouveau binaire API.

Livrable de sortie : ancien et nouveau binaires fonctionnent avec le schema etendu.

### Phase 3 - Deploiement blue/green

1. conserver le conteneur bleu et son image N-1 en fonctionnement ;
2. demarrer le conteneur vert sous un autre nom et un autre port ;
3. connecter le vert au reseau etendu ;
4. lancer health checks, smoke tests de lecture et ecritures synthetiques identifiables ;
5. envoyer un faible pourcentage du trafic ou un seul consommateur pilote vers le vert ;
6. surveiller erreurs HTTP, latence, connexions DB, verrous et ecarts de donnees ;
7. basculer le reste du trafic seulement si les seuils sont respectes.

Rollback applicatif : rebascule immediate vers le bleu. Le schema etant additif, aucun rollback DB n'est necessaire.

### Phase 4 - Migration progressive des consommateurs

Pour chaque logiciel :

1. deployer sa version compatible v2 ;
2. effectuer un test fonctionnel signe sur une ligne ou un poste pilote ;
3. surveiller un cycle de production complet ;
4. marquer le consommateur comme migre dans la matrice ;
5. conserver v1 pour les consommateurs restants.

### Phase 5 - Migration et reconciliation des donnees

1. lancer la migration historique par lots ;
2. traiter les rejets ;
3. comparer les totaux et echantillons fonctionnels ;
4. verifier le double-write ou les adaptateurs ;
5. obtenir une validation metier ecrite.

### Phase 6 - Contraction differee

Seulement apres la periode d'observation convenue et lorsque les journaux montrent zero appel v1 :

1. annoncer la date de retrait ;
2. desactiver v1 de facon reversible pendant une periode test ;
3. archiver la table historique et conserver un export verifie ;
4. supprimer les anciennes routes, colonnes et tables dans un deploiement distinct ;
5. ne jamais coupler cette suppression avec une nouvelle fonctionnalite.

## 8. Criteres Go / No-Go

### Go

- sauvegarde restauree avec succes et chronometree ;
- 100 % des consommateurs critiques inventories ;
- tests de contrat v1 et v2 au vert ;
- ancien binaire compatible avec le schema etendu ;
- aucune ligne `Tolerance` sans `UnitUid` valide avant ajout de la contrainte obligatoire ;
- taux de rapprochement automatique des mesures accepte par le metier ;
- rejets documentes et sans perte silencieuse ;
- image N-1 disponible et bascule retour testee ;
- health checks et tableaux de bord operationnels ;
- responsables presents pendant la fenetre.

### No-Go immediat

- script contenant `DROP TABLE MeasurementTraceabilities` ou `DROP COLUMN Tolerances.Unit` avant archivage et validation ;
- ecart non explique entre le nombre de lignes source et `migre + rejete` ;
- consommateur critique non teste ;
- absence de restauration prouvee ;
- ancien binaire incompatible avec le schema etendu ;
- verrouillage depassant la fenetre autorisee ;
- impossibilite de rebascule vers N-1.

## 9. Tests minimaux

- tests de contrat sur chaque ancienne route singuliere et nouvelle route plurielle ;
- tests des anciens et nouveaux noms de parametres de query string ;
- tests de serialisation objet/collection ;
- creation de mesure v1, v2, batch atomique et batch best-effort ;
- erreur controlee pour incoherence produit/tolerance ;
- lecture des mesures historiques apres expansion ;
- validation de toutes les valeurs d'unite et de mode existantes ;
- test de charge sur les recherches par produit/operation ;
- test de panne du conteneur vert et bascule vers le bleu ;
- test de relance du migrateur apres interruption.

## 10. Ordre de travail recommande

1. corriger la strategie de migration destructive ;
2. retablir la compatibilite v1 et les alias ;
3. ajouter les tests de contrat ;
4. construire l'inventaire des logiciels ;
5. developper le migrateur idempotent et la table de rejets ;
6. modifier le pipeline en blue/green avec health check et conservation N-1 ;
7. repeter la procedure complete sur une copie de production ;
8. seulement ensuite planifier le passage en production.

