using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProductCatalog.Api.Data;
using ProductCatalog.Api.Models;
using Xunit;

namespace ProductCatalog.Tests;

public class ProductIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProductIntegrationTests(WebApplicationFactory<Program> factory)
    {
        // Récupération de la chaîne de connexion (priorité à la variable d'env CI)
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__CatalogDbConnection")
            ?? "Server=localhost,1433;Database=CatalogTestDb;User Id=sa;Password=DevOpsPass2026!#;TrustServerCertificate=True;";

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.RemoveAll(typeof(IDbContextOptionsConfiguration<AppDbContext>));

                // Connexion directe au vrai moteur SQL Server
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });
            });
        });
    }

    // Fonction unique de test d'intégration : Persistance réelle dans la BD SQL Server
    [Fact]
    public async Task AddProduct_And_VerifyPersistenceInRealDatabase()
    {
        // S'assurer que la base de données et la table Products existent sur SQL Server
        using (var initScope = _factory.Services.CreateScope())
        {
            var initDb = initScope.ServiceProvider.GetRequiredService<AppDbContext>();
            await initDb.Database.EnsureCreatedAsync();
        }

        var client = _factory.CreateClient();
        var uniqueName = "Serveur-Rack-" + Guid.NewGuid().ToString().Substring(0, 6);
        var product = new Product { Name = uniqueName, Price = 1299.99m };

        // 1. Insertion via l'endpoint API
        var postResponse = await client.PostAsJsonAsync("/api/products", product);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        // 2. Vérification directe de persistance en interrogeant la BD SQL Server
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var persistedProduct = await db.Products.FirstOrDefaultAsync(p => p.Name == uniqueName);

        Assert.NotNull(persistedProduct);
        Assert.Equal(1299.99m, persistedProduct.Price);
    }
}











