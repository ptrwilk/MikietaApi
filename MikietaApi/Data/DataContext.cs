using Microsoft.EntityFrameworkCore;
using MikietaApi.Data.Entities;

namespace MikietaApi.Data;

public class DataContext : DbContext
{
    public DbSet<IngredientEntity> Ingredients { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<OrderOrderedProductEntity> OrderOrderedProducts { get; set; }
    public DbSet<ReservationEntity> Reservations { get; set; }
    public DbSet<OrderedProductEntity> OrderedProducts { get; set; }
    public DbSet<OrderedIngredientEntity> OrderedIngredients { get; set; }
    public DbSet<OrderedProductOrderedIngredientEntity> OrderedProductOrderedIngredients { get; set; }
    
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductEntity>()
            .HasMany(x => x.Ingredients)
            .WithMany(x => x.Products)
            .UsingEntity(x => x.ToTable("ProductIngredient"));

        modelBuilder.Entity<OrderEntity>()
            .Property(x => x.Number)
            .IsUnicode()
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<OrderEntity>()
            .Property(x => x.DeliveryMethod)
            .HasConversion<string>();
        modelBuilder.Entity<OrderEntity>()
            .Property(x => x.PaymentMethod)
            .HasConversion<string>();
        modelBuilder.Entity<OrderEntity>()
            .Property(x => x.Status)
            .HasConversion<string>();
        
        modelBuilder.Entity<OrderOrderedProductEntity>()
            .HasKey(op => new { op.OrderId, ProductId = op.OrderedProductId });
        modelBuilder.Entity<OrderOrderedProductEntity>()
            .HasOne(op => op.Order)
            .WithMany(o => o.OrderOrderedProducts)
            .HasForeignKey(op => op.OrderId);
        modelBuilder.Entity<OrderOrderedProductEntity>()
            .HasOne(op => op.OrderedProduct)
            .WithMany(p => p.OrderOrderedProducts)
            .HasForeignKey(op => op.OrderedProductId);
        
        modelBuilder.Entity<ReservationEntity>()
            .Property(x => x.Number)
            .IsUnicode()
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<IngredientEntity>()
            .Ignore(x => x.Prices);
        
        modelBuilder.Entity<OrderedProductOrderedIngredientEntity>()
            .HasKey(op => new { op.OrderedProductId, op.OrderedIngredientId });
        modelBuilder.Entity<OrderedProductOrderedIngredientEntity>()
            .HasOne(op => op.OrderedProduct)
            .WithMany(o => o.OrderedProductOrderedIngredients)
            .HasForeignKey(op => op.OrderedProductId);
        modelBuilder.Entity<OrderedProductOrderedIngredientEntity>()
            .HasOne(op => op.OrderedIngredient)
            .WithMany(p => p.OrderedProductOrderedIngredients)
            .HasForeignKey(op => op.OrderedIngredientId);
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added).ToList();
        
        var maxProductsIndex = Products.Any() ? Products.Max(x => x.Index) + 1 : 1;
        var maxIngredientsIndex = Ingredients.Any() ? Ingredients.Max(x => x.Index) + 1 : 1;

        foreach (var entry in entries)
        {
            if (entry.Entity is OrderEntity orderEntity)
            {
                var number = Orders.Any() ? Orders.Max(x => x.Number) + 1 : 1;

                orderEntity.Number = number;
            }
            else if (entry.Entity is ReservationEntity reservationEntity)
            {
                var number = Reservations.Any() ? Reservations.Max(x => x.Number) + 1 : 1;

                reservationEntity.Number = number;              
            }
            else if (entry.Entity is ProductEntity productEntity)
            {
                productEntity.Index = maxProductsIndex;
                maxProductsIndex++;
            }
            if (entry.Entity is IngredientEntity ingredientEntity)
            {
                ingredientEntity.Index = maxIngredientsIndex;
                maxIngredientsIndex++;
            }
        }
        
        return base.SaveChanges();
    }
}