# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copier les fichiers de projet et restaurer les d�pendances
COPY ["Api.Data/Api.Data.csproj", "Api.Data/"]
COPY ["DataLib/DataLib.csproj", "DataLib/"]
COPY ["Migrations/Migrations.csproj", "Migrations/"]
RUN dotnet restore "Api.Data/Api.Data.csproj"

# Copier tout le code source
COPY . .

# Build du projet
WORKDIR "/src/Api.Data"
RUN dotnet build "Api.Data.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "Api.Data.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Cr�er le r�pertoire pour les logs
RUN mkdir -p /app/Logs

# Copier les fichiers publi�s
COPY --from=publish /app/publish .

# Exposer le port (ajustez selon votre configuration)
EXPOSE 10000

# Variables d'environnement par d�faut
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

# Point d'entr�e
ENTRYPOINT ["dotnet", "Api.Data.dll"]
