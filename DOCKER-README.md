# API Data - Docker

## Construction de l'image Docker

### Option 1: Construire l'image localement

```bash
# Construire l'image
docker build -t api-data:latest .

# Exécuter le conteneur
docker run -d -p 5000:8080 --name api-data \
  -e ConnectionStrings__DefaultConnection="Server=your-mysql-host;Database=production;User=root;Password=your_password;" \
  api-data:latest
```

### Option 2: Utiliser Docker Compose

```bash
# Démarrer tous les services (API + MySQL)
docker-compose up -d

# Arrêter les services
docker-compose down

# Voir les logs
docker-compose logs -f api-data
```

## Exporter/Importer l'image

### Exporter l'image pour un autre PC

```bash
# Sauvegarder l'image dans un fichier tar
docker save -o api-data-image.tar api-data:latest

# Transférer le fichier api-data-image.tar vers l'autre PC
```

### Importer l'image sur un autre PC

```bash
# Charger l'image depuis le fichier tar
docker load -i api-data-image.tar

# Vérifier que l'image est chargée
docker images | grep api-data

# Exécuter le conteneur
docker run -d -p 5000:8080 --name api-data \
  -e ConnectionStrings__DefaultConnection="Server=your-mysql-host;Database=production;User=root;Password=your_password;" \
  api-data:latest
```

## Configuration

### Variables d'environnement importantes

- `ASPNETCORE_ENVIRONMENT`: Environnement (Development, Production)
- `ConnectionStrings__DefaultConnection`: Chaîne de connexion MySQL
- `ASPNETCORE_URLS`: URLs d'écoute (par défaut: http://+:8080)

### Modifier la configuration

Vous pouvez modifier les variables d'environnement dans le fichier `docker-compose.yml` ou les passer directement avec `-e` lors du `docker run`.

## Accès à l'API

- API: http://localhost:5000
- Swagger UI: http://localhost:5000/swagger

## Volumes

- `./Logs:/app/Logs`: Les logs de l'application sont persistés localement

## Commandes utiles

```bash
# Voir les conteneurs en cours d'exécution
docker ps

# Voir les logs
docker logs -f api-data

# Entrer dans le conteneur
docker exec -it api-data /bin/bash

# Supprimer le conteneur
docker rm -f api-data

# Supprimer l'image
docker rmi api-data:latest
```

## Dépannage

### Le conteneur ne démarre pas
```bash
# Vérifier les logs
docker logs api-data

# Vérifier la configuration
docker inspect api-data
```

### Problème de connexion à la base de données
- Vérifiez que la chaîne de connexion est correcte
- Assurez-vous que le serveur MySQL est accessible depuis le conteneur
- Si vous utilisez docker-compose, le nom d'hôte MySQL est `mysql`
