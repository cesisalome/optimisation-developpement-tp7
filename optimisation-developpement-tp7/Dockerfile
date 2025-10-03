# Consultez https://aka.ms/customizecontainer pour savoir comment personnaliser votre conteneur de débogage et comment Visual Studio utilise ce Dockerfile pour générer vos images afin d’accélérer le débogage.

# Cette étape permet de publier le projet de service à copier dans la phase finale
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["optimisation-developpement-tp7.csproj", "."]
RUN dotnet restore "./optimisation-developpement-tp7.csproj"
COPY . .
RUN dotnet publish "./optimisation-developpement-tp7.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Cette phase est utilisée en production ou lors de l’exécution à partir de VS en mode normal (par défaut quand la configuration de débogage n’est pas utilisée)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "optimisation-developpement-tp7.dll"]