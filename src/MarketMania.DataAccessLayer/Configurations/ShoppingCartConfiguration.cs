using MarketMania.DataAccessLayer.Configurations.Common;
using MarketMania.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketMania.DataAccessLayer.Configurations;

internal class ShoppingCartConfiguration : BaseEntityConfiguration<ShoppingCart>
{
    public override void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        //builder.HasOne(typeof(ApplicationUser))
        //    .WithMany("ShoppingCarts")
        //    .HasForeignKey("UserId")
        //    .HasConstraintName("FK_ShoppingCarts_UserId")
        //    .IsRequired()
        //    .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("ShoppingCarts");
        base.Configure(builder);
    }
}