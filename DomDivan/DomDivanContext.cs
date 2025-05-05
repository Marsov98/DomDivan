using Microsoft.EntityFrameworkCore;
using DomDivan.Models;

namespace DomDivan;

public class DomDivanContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Sofa> Sofas { get; set; }
    public DbSet<Armchair> Armchairs { get; set; }
    public DbSet<Bed> Beds { get; set; }
    public DbSet<Variant> Variants { get; set; }
    public DbSet<ColorVariant> Colors { get; set; }
    public DbSet<Cloth> Cloths { get; set; }
    public DbSet<SofaType> SofaTypes { get; set; }
    public DbSet<Filler> Fillers { get; set; }
    public DbSet<FoldingMechanism> FoldingMechanisms { get; set; }
    public DbSet<PhotoProduct> Photos { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Supply> Supplies { get; set; }
    public DbSet<ProductInSupply> ProductsInSupply { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = @"..\..\..\dom_divan.db";
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка наследования TPT (Table Per Type)
        modelBuilder.Entity<Product>().ToTable("Products");
        modelBuilder.Entity<Sofa>().ToTable("Sofas");
        modelBuilder.Entity<Armchair>().ToTable("Armchairs");
        modelBuilder.Entity<Bed>().ToTable("Beds");

        // Настройка связей для Product
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.Variants)
            .WithOne(v => v.Product)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Настройка связей для Variant
        modelBuilder.Entity<Variant>()
            .HasOne(v => v.Color)
            .WithMany()
            .HasForeignKey(v => v.ColorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Variant>()
            .HasOne(v => v.Cloth)
            .WithMany()
            .HasForeignKey(v => v.ClothId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Variant>()
            .HasOne(v => v.SofaType)
            .WithMany()
            .HasForeignKey(v => v.SofaTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Variant>()
            .HasMany(v => v.Photos)
            .WithOne(p => p.Variant)
            .HasForeignKey(p => p.VariantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Настройка связей для Order
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Настройка связей для OrderItem
        modelBuilder.Entity<OrderItem>()
            .HasOne(i => i.Variant)
            .WithMany()
            .HasForeignKey(i => i.VariantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Настройка значений по умолчанию
        modelBuilder.Entity<Order>()
            .Property(o => o.OrderDate)
            .HasDefaultValueSql("GETDATE()");

        // Индексы для улучшения производительности
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.CategoryId);

        modelBuilder.Entity<Variant>()
            .HasIndex(v => v.ProductId);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.Status);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.PhoneNumber);
    }
}
