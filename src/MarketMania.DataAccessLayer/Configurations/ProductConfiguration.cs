using MarketMania.DataAccessLayer.Configurations.Common;
using MarketMania.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketMania.DataAccessLayer.Configurations;

internal class ProductConfiguration : BaseEntityConfiguration<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.Name).HasMaxLength(255).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(4000).IsRequired();

        builder.Property(p => p.Title).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Subtitle).HasMaxLength(100).IsRequired(false);

        builder.Property(p => p.Price).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.TotalPrice).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.ShippingCost).HasPrecision(5, 2).IsRequired(false);

        builder.Property(p => p.ImageUrl).HasMaxLength(2048).IsRequired(false);
        builder.Property(p => p.SerialNumber).HasMaxLength(50).IsUnicode(false);

        builder.Property(p => p.IsPublished).ValueGeneratedOnAdd().HasDefaultValueSql("((0))");
        builder.Property(p => p.IsAvailable).ValueGeneratedOnAdd().HasDefaultValueSql("((1))");
        builder.Property(p => p.IsFeatured).ValueGeneratedOnAdd().HasDefaultValueSql("((0))");

        //builder.Property(p => p.Tags).HasArrayConversion().HasColumnType("NVARCHAR(MAX)").IsRequired();
        builder.Property(p => p.Barcode).HasMaxLength(100).IsRequired();

        builder.Property(p => p.SeoTitle).HasMaxLength(255).IsRequired(false);
        builder.Property(p => p.SeoDescription).HasMaxLength(4000).IsRequired(false);
        builder.Property(p => p.WarehouseLocation).HasMaxLength(100).IsRequired();

        builder.HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .HasConstraintName("FK_Products_Brands")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .HasConstraintName("FK_Products_Categories")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId)
            .HasConstraintName("FK_Products_Suppliers")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Promotion)
            .WithMany(p => p.Products)
            .HasForeignKey(p => p.PromotionId)
            .HasConstraintName("FK_Products_Promotions")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.Name)
            .IsClustered(false)
            .HasDatabaseName("IX_Products_Name");

        builder.HasIndex(p => new { p.BrandId, p.CategoryId, p.SupplierId, p.Name })
            .IsClustered(false)
            .IsUnique()
            .HasDatabaseName("IX_Products_UniqueName");

        builder.HasIndex(p => p.SerialNumber)
            .IsClustered(false)
            .IsUnique()
            .HasDatabaseName("IX_Products_UniqueSerialNumber");

        builder.HasIndex(p => p.PromotionId)
            .IsClustered(false)
            .HasDatabaseName("IX_Products_PromotionId");

        builder.HasIndex(p => p.Barcode)
            .IsClustered(false)
            .IsUnique()
            .HasDatabaseName("IX_Products_Barcode");

        builder.ToTable("Products");
        base.Configure(builder);
    }
}