# Environnement local API Data V2

Cette stack est indépendante de la production. Elle démarre :

- MySQL 8.0.45 avec une copie logique de `db_production_v2` ;
- l'API V2 construite depuis le code courant ;
- phpMyAdmin 5.2.1.

Tous les ports sont liés à `127.0.0.1` et ne sont donc pas exposés au réseau.

## 1. Exporter la base V2 depuis le serveur de production

Depuis `cmd.exe` sur le serveur de production :

```bat
cd /d C:\Users\adminprod\Desktop\Database
docker exec mysql-prod sh -c "exec mysqldump -uroot -p$MYSQL_ROOT_PASSWORD --single-transaction --quick --routines --triggers --events --hex-blob --set-gtid-purged=OFF --no-tablespaces --default-character-set=utf8mb4 db_production_v2" > db_production_v2.sql
```

Cette commande est en lecture seule pour MySQL et produit un instantané cohérent
des tables InnoDB sans arrêter l'API de production.

Transférer ensuite le fichier de manière sécurisée vers :

```text
local-v2\import\01_db_production_v2.sql
```

Le dump contient des données de production : ne pas le commiter, ne pas le
joindre à un ticket et ne pas le déposer dans un emplacement partagé non protégé.

## 2. Démarrer la stack locale

Depuis PowerShell à la racine du dépôt :

```powershell
.\Scripts\Start-LocalV2.ps1
```

Au premier démarrage, MySQL importe automatiquement le dump. Selon sa taille,
cette étape peut prendre plusieurs minutes.

## 3. Accéder aux services

```text
API        http://127.0.0.1:11000
Swagger    http://127.0.0.1:11000/swagger
phpMyAdmin http://127.0.0.1:9081
MySQL      127.0.0.1:23306
```

Identifiants phpMyAdmin : ceux de `.env.local-v2` (`LOCAL_V2_MYSQL_USER` et
`LOCAL_V2_MYSQL_PASSWORD`).

## Arrêter sans supprimer les données

```powershell
docker compose --env-file .env.local-v2 -f docker-compose.local-v2.yml stop
```

## Redémarrer sans reconstruire l'API

```powershell
.\Scripts\Start-LocalV2.ps1 -NoBuild
```

## Réimporter un nouveau dump

L'import automatique ne s'exécute que sur un volume MySQL vide. Après avoir
arrêté la stack et remplacé le dump, la commande suivante supprime uniquement le
volume MySQL de cette stack locale :

```powershell
docker compose --env-file .env.local-v2 -f docker-compose.local-v2.yml down --volumes
```

Cette commande détruit la copie locale de la base. Elle n'accède jamais à la
base de production. Relancer ensuite `Start-LocalV2.ps1`.
