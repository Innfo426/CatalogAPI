using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProductCatalog.Api.Data;
using ProductCatalog.Api.Models;
using Xunit;

namespace ProductCatalog.Tests;

public class ProductUnitTests
{
    private WebApplicationFactory<Program> CreateFactoryWithInMemoryDb(string dbName)
    {
        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Remplacer SQL Server par une base InMemory isolée par test
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });
            });
        });
    }

    [Fact]
    public async Task GetAllProducts_ReturnsOkAndProductList()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var factory = CreateFactoryWithInMemoryDb(dbName);
        var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Products.AddRange(
                new Product { Name = "Clavier Mécanique", Price = 89.99m },
                new Product { Name = "Souris Sans Fil", Price = 45.50m }
            );
            await db.SaveChangesAsync();
        }

        // Act
        var response = await client.GetAsync("/api/products");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        Assert.NotNull(products);
        Assert.Equal(2, products.Count);
    }

    [Fact]
    public async Task CreateProduct_ReturnsCreatedAndPersistsInMemory()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var factory = CreateFactoryWithInMemoryDb(dbName);
        var client = factory.CreateClient();

        var newProduct = new Product
        {
            Name = "Écran 27 pouces",
            Price = 299.00m
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/products", newProduct);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdProduct = await response.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(createdProduct);
        Assert.Equal("Écran 27 pouces", createdProduct.Name);
        Assert.True(createdProduct.Id > 0);
    }
}














