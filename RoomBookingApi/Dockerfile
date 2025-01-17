# Étape 1 : Utiliser une image de base pour le SDK .NET
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copier le fichier projet et restaurer les dépendances
COPY *.csproj ./
RUN dotnet restore

# Copier tous les fichiers et compiler l'application
COPY . ./
RUN dotnet publish -c Release -o out

# Étape 2 : Utiliser une image runtime légère pour exécuter l'application
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .

# Point d'entrée pour lancer l'application
ENTRYPOINT ["dotnet", "RoomBookingApi.dll"]