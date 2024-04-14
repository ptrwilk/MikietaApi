using Microsoft.EntityFrameworkCore;
using MikietaApi.Data.Entities;

namespace MikietaApi.Data;

public class DataContext : DbContext
{
    public DbSet<IngredientEntity> Ingredients { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
    
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductEntity>()
            .HasMany(x => x.Ingredients)
            .WithMany(x => x.Products)
            .UsingEntity(x => x.ToTable("ProductIngredient"));

        modelBuilder.Entity<ProductEntity>()
            .Property(x => x.ProductType)
            .HasConversion<string>();
    }
}