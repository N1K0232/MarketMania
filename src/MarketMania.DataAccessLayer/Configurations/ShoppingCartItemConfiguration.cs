using MarketMania.DataAccessLayer.Configurations.Common;
using MarketMania.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketMania.DataAccessLayer.Configurations;

internal class ShoppingCartItemConfiguration : BaseEntityConfiguration<ShoppingCartItem>
{
    public override void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
    {
        builder.HasOne(s => s.ShoppingCart)
            .WithMany(s => s.ShoppingCartItems)
            .HasForeignKey(s => s.ShoppingCartId)
            .HasConstraintName("FK_ShoppingCartItems_ShoppingCart")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Product)
            .WithMany(p => p.ShoppingCartItems)
            .HasForeignKey(s => s.ProductId)
            .HasConstraintName("FK_ShoppingCartItems_Product")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(s => s.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.Quantity).IsRequired();

        builder.HasIndex(s => new { s.ShoppingCartId, s.ProductId })
            .HasDatabaseName("IX_ShoppingCart_Product")
            .IsClustered(false)
            .IsUnique();

        builder.ToTable("ShoppingCartItems");
        base.Configure(builder);
    }
}