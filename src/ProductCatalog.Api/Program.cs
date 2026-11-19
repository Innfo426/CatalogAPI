using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Data;
using ProductCatalog.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuration de la base de données SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuration de Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Application automatique des migrations au démarrage (hors environnement de test InMemory)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.IsRelational())
    {
        db.Database.Migrate();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Endpoints de l'API ProductCatalog
app.MapGet("/api/products", async (AppDbContext db) =>
    await db.Products.ToListAsync())
    .WithName("GetAllProducts");

app.MapGet("/api/products/{id:int}", async (int id, AppDbContext db) =>
    await db.Products.FindAsync(id) is Product product
        ? Results.Ok(product)
        : Results.NotFound(new { message = $"Aucun produit trouvé avec l'identifiant {id}." }))
    .WithName("GetProductById");

app.MapPost("/api/products", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/api/products/{product.Id}", product);
})
.WithName("CreateProduct");

// Action de Suppression ajoutée par mon collaborateur (Partie 6)
app.MapDelete("/api/products/{id:int}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null)
    {
        return Results.NotFound(new
        {
            message = $"Aucun produit trouvé avec l'identifiant {id}."
        });
    }

    db.Products.Remove(product);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeleteProduct");

app.Run();

// Exposition de Program pour le projet de tests (TP3 - Partie 1.2)
public partial class Program { }






