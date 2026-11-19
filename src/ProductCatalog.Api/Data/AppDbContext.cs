<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Models;
namespace ProductCatalog.Api.Data;
public class AppDbContext : DbContext
{
public AppDbContext(DbContextOptions<AppDbContext> options) :
base(options) { }
public DbSet<Product> Products => Set<Product>();
protected override void OnModelCreating(ModelBuilder
modelBuilder)
{
base.OnModelCreating(modelBuilder);
modelBuilder.Entity<Product>().Property(p =>
p.Name).IsRequired().HasMaxLength(30);
modelBuilder.Entity<Product>().Property(p =>
p.Price).HasPrecision(10, 2);
}
}

=======
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Models;
namespace ProductCatalog.Api.Data;
public class AppDbContext : DbContext
{
public AppDbContext(DbContextOptions<AppDbContext> options) :
base(options) { }
public DbSet<Product> Products => Set<Product>();
protected override void OnModelCreating(ModelBuilder
modelBuilder)
{
base.OnModelCreating(modelBuilder);
modelBuilder.Entity<Product>().Property(p =>
p.Name).IsRequired().HasMaxLength(30);
modelBuilder.Entity<Product>().Property(p =>
p.Price).HasPrecision(10, 2);
}
}

>>>>>>> 5f96a36d72ba71a3a5e223654d1511ab0972f9be
