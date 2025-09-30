using System;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<StockAdjustment> StockAdjustments { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //relations
        modelBuilder.Entity<Category>()
        .HasMany(c => c.Products)
        .WithOne(p => p.Category)
        .HasForeignKey(p => p.CategoryId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Supplier>()
        .HasMany(s => s.Products)
        .WithOne(p => p.Supplier)
        .HasForeignKey(p => p.SupplierId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
        .HasMany(p => p.Sales)
        .WithOne(s => s.Product)
        .HasForeignKey(s => s.ProductId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
        .HasMany(p => p.StockAdjustments)
        .WithOne(sa => sa.Product)
        .HasForeignKey(sa => sa.ProductId)
        .OnDelete(DeleteBehavior.Restrict);

        //anotations

        //category
        modelBuilder.Entity<Category>()
        .Property(c => c.Id)
        .ValueGeneratedOnAdd();

        modelBuilder.Entity<Category>()
        .Property(p => p.Name)
        .HasMaxLength(100)
        .IsRequired();

        //product
        modelBuilder.Entity<Product>()
        .Property(p => p.Id)
        .ValueGeneratedOnAdd();

        modelBuilder.Entity<Product>()
        .Property(p => p.Name)
        .HasMaxLength(100)
        .IsRequired();

        modelBuilder.Entity<Product>()
        .Property(p => p.Price)
        .IsRequired();

        modelBuilder.Entity<Product>()
        .Property(p => p.QuantityInStock)
        .IsRequired();

        modelBuilder.Entity<Product>()
        .Property(p => p.CategoryId)
        .IsRequired();

        modelBuilder.Entity<Product>()
        .Property(p => p.SupplierId)
        .IsRequired();

        //sale
        modelBuilder.Entity<Sale>()
        .Property(s => s.Id)
        .ValueGeneratedOnAdd()
        .IsRequired();

        modelBuilder.Entity<Sale>()
        .Property(s => s.ProductId)
        .IsRequired();

        modelBuilder.Entity<Sale>()
        .Property(s => s.QuantitySold)
        .IsRequired();


        //StockAdjustment
        modelBuilder.Entity<StockAdjustment>()
        .Property(sa => sa.Id)
        .ValueGeneratedOnAdd()
        .IsRequired();

        modelBuilder.Entity<StockAdjustment>()
        .Property(sa => sa.ProductId)
        .IsRequired();

        modelBuilder.Entity<StockAdjustment>()
        .Property(sa => sa.AdjustmentAmount)
        .IsRequired();

        modelBuilder.Entity<StockAdjustment>()
        .Property(sa => sa.Reason)
        .IsRequired();

        //Supplier
        modelBuilder.Entity<Supplier>()
        .Property(s => s.Id)
        .ValueGeneratedOnAdd();

        modelBuilder.Entity<Supplier>()
        .Property(s => s.Name)
        .HasMaxLength(150)
        .IsRequired();

        modelBuilder.Entity<Supplier>()
        .Property(s => s.Phone)
        .HasMaxLength(11)
        .IsRequired();















    }
    
}
