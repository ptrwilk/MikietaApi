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
    public DbSet<SettingEntity> Settings { get; set; }
    public DbSet<PizzaSizeEntity> PizzaSizes { get; set; }
    
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
        
        modelBuilder.Entity<OrderedProductEntity>()
            .Property(x => x.ProductType)
            .HasConversion<string>();
        modelBuilder.Entity<OrderedProductEntity>()
            .Property(x => x.PizzaType)
            .HasConversion<string>();

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
        modelBuilder.Entity<ReservationEntity>()
            .Property(x => x.Status)
            .HasConversion<string>();

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

        modelBuilder.Entity<SettingEntity>()
            .HasKey(x => x.Key);
        
        modelBuilder.Entity<PizzaSizeEntity>()
            .Property(x => x.Size)
            .HasConversion<string>();
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added).ToList();
        
        var maxProductsIndex = Products.Any() ? Products.Max(x => x.Index) + 1 : 1;
        var maxIngredientsIndex = Ingredients.Any() ? Ingredients.Max(x => x.Index) + 1 : 1;
        var maxOrderedIngredient = OrderedIngredients.Any() ? OrderedIngredients.Max(x => x.Index) + 1 : 1;
        var maxOrderedProducts = OrderedProducts.Any() ? OrderedProducts.Max(x => x.Index) + 1 : 1;

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
            else if (entry.Entity is IngredientEntity ingredientEntity)
            {
                ingredientEntity.Index = maxIngredientsIndex;
                maxIngredientsIndex++;
            }
            else if (entry.Entity is OrderedIngredientEntity orderedIngredientEntity)
            {
                orderedIngredientEntity.Index = maxOrderedIngredient;
                maxOrderedIngredient++;
            }
            else if (entry.Entity is OrderedProductEntity orderedProductEntity)
            {
                orderedProductEntity.Index = maxOrderedProducts;
                maxOrderedProducts++;
            }
        }
        
        return base.SaveChanges();
    }
    
    public static T? GetValue<T>(SettingEntity[] settings, string key)
    {
        var value = settings.Single(x => x.Key == key).Value;

        if (string.IsNullOrEmpty(value))
        {
            return default;
        }
        
        if (typeof(T) == typeof(TimeSpan))
        {
            return (T)(object)TimeSpan.Parse(value ?? "");
        }

        if (typeof(T) == typeof(string))
        {
            return (T)(object)value;
        }
        
        if (typeof(T) == typeof(double) || typeof(T) == typeof(double?))
        {
            return (T)(object)double.Parse(value);
        }

        throw new ArgumentException($"Case not specified for type {typeof(T)}");
    }

    public T? GetValue<T>(string key)
    {
        var settings = Settings.Single(x => x.Key == key);
        
        return GetValue<T>(new [] {settings}, key);
    }
}