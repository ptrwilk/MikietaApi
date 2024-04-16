using Microsoft.EntityFrameworkCore;
using MikietaApi.Data.Entities;

namespace MikietaApi.Data;

public class DataContext : DbContext
{
    public DbSet<IngredientEntity> Ingredients { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<OrderEntity> Orders { get; set; }
    
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

        modelBuilder.Entity<OrderEntity>()
            .Property(x => x.Number)
            .IsUnicode()
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<OrderEntity>()
            .HasMany(x => x.Products)
            .WithMany(x => x.Orders)
            .UsingEntity(x => x.ToTable("OrderProduct"));
        modelBuilder.Entity<OrderEntity>()
            .Property(x => x.DeliveryMethod)
            .HasConversion<string>();
        modelBuilder.Entity<OrderEntity>()
            .Property(x => x.PaymentMethod)
            .HasConversion<string>();
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added && e.Entity is OrderEntity).ToList();

        foreach (var entry in entries)
        {
            var entity = (OrderEntity)entry.Entity;

            var number = Orders.Any() ? Orders.Max(x => x.Number) + 1 : 1;

            entity.Number = number;
        }
        
        return base.SaveChanges();
    }
}