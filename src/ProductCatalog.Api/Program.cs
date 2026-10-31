using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Data;
using ProductCatalog.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuration du contexte de base de données avec SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- ENDPOINTS DE L'API MIS À JOUR (FIN TP2) ---

// 1. Récupérer tous les produits (Route renommée - Partie 4)
app.MapGet("/api/get-all-products", async (AppDbContext db) =>
    await db.Products.AsNoTracking().ToListAsync())
    .WithName("GetAllProducts");

// 2. Récupérer un produit par son identifiant unique (Partie 3)
app.MapGet("/api/get-product/{id:int}", async (int id, AppDbContext db) => 
{ 
    var product = await db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id); 
    if (product is null) 
    { 
        return Results.NotFound(new { message = $"Aucun produit trouvé avec l'identifiant {id}." }); 
    } 
    return Results.Ok(product); 
})
.WithName("GetProductById");

// 3. Ajouter un nouveau produit (Route renommée - Partie 4)
app.MapPost("/api/create-product", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/api/get-product/{product.Id}", product);
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

// Test des pull requests !!! (Partie 8)

app.Run();


































