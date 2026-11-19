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

public class ProductUnitTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProductUnitTests(WebApplicationFactory<Program> factory)
    {
        // Un nom unique fixe pour cette classe de test afin que POST et GET partagent la même base
        var dbName = "UnitTestingDb_" + Guid.NewGuid();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.RemoveAll(typeof(IDbContextOptionsConfiguration<AppDbContext>));

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });
            });
        });
    }

    // 1. Test unitaire : Ajout d'un produit (POST /api/create-product)
    [Fact]
    public async Task AddProduct_ReturnsCreated_WhenProductIsValid()
    {
        var client = _factory.CreateClient();
        var newProduct = new Product { Name = "Clavier Mecanique", Price = 89.50m };

        var response = await client.PostAsJsonAsync("/api/create-product", newProduct);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdProduct = await response.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(createdProduct);
        Assert.Equal("Clavier Mecanique", createdProduct.Name);
        Assert.Equal(89.50m, createdProduct.Price);
    }

    // 2. Test unitaire : Listing des produits (GET /api/get-all-products)
    [Fact]
    public async Task GetAllProducts_ReturnsOkAndProductList()
    {
        var client = _factory.CreateClient();

        // Pré-insertion d'un produit de test
        await client.PostAsJsonAsync("/api/create-product", new Product { Name = "Ecran 27 pouces", Price = 250.00m });

        var response = await client.GetAsync("/api/get-all-products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        Assert.NotNull(products);
        Assert.NotEmpty(products);
        Assert.Contains(products, p => p.Name == "Ecran 27 pouces");
    }
}








