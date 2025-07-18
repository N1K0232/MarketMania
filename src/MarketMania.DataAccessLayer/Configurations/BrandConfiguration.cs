using MarketMania.DataAccessLayer.Configurations.Common;
using MarketMania.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketMania.DataAccessLayer.Configurations;

internal class BrandConfiguration : BaseEntityConfiguration<Brand>
{
    public override void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.Property(b => b.Name).HasMaxLength(255).IsRequired();
        builder.Property(b => b.City).HasMaxLength(50).IsRequired();

        builder.ToTable("Brands");
        base.Configure(builder);
    }
}