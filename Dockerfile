# 1. Adapter la version (8.0 ou 10.0 selon votre .csproj)
FROM ://microsoft.com AS base
WORKDIR /app
EXPOSE 8080
USER app

FROM ://microsoft.com AS build
WORKDIR /src

# Copie du csproj sur une seule ligne (corrigé)
COPY ["src/ProductCatalog.Api/ProductCatalog.Api.csproj", "ProductCatalog.Api/"]
RUN dotnet restore "ProductCatalog.Api/ProductCatalog.Api.csproj"

# Copie du reste des sources
COPY src/ProductCatalog.Api/ ProductCatalog.Api/
WORKDIR "/src/ProductCatalog.Api"

# Compilation sans le flag strict --no-restore pour laisser le compilateur valider les assets
RUN dotnet build "ProductCatalog.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductCatalog.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductCatalog.Api.dll"]
