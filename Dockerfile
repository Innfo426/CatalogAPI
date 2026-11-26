FROM ://microsoft.com AS base
WORKDIR /app
EXPOSE 8080
USER app

FROM ://microsoft.com AS build
WORKDIR /src

COPY ["src/ProductCatalog.Api/ProductCatalog.Api.csproj", "ProductCatalog.Api/"]
RUN dotnet restore "ProductCatalog.Api/ProductCatalog.Api.csproj"

COPY src/ProductCatalog.Api/ ProductCatalog.Api/
WORKDIR "/src/ProductCatalog.Api"

RUN dotnet build "ProductCatalog.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductCatalog.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductCatalog.Api.dll"]
