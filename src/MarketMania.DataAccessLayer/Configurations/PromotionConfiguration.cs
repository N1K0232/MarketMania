using MarketMania.DataAccessLayer.Configurations.Common;
using MarketMania.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketMania.DataAccessLayer.Configurations;

internal class PromotionConfiguration : BaseEntityConfiguration<Promotion>
{
    public override void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.Property(p => p.Name).HasMaxLength(255).IsRequired();
        builder.Property(p => p.DiscountPercentage).HasPrecision(5, 2).IsRequired();

        builder.Property(p => p.StartDate).IsRequired();
        builder.Property(p => p.EndDate).IsRequired();

        builder.Property(p => p.IsActive).HasDefaultValueSql("((1))");

        builder.ToTable("Promotions");
        base.Configure(builder);
    }
}