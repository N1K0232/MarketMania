using MarketMania.DataAccessLayer.Configurations.Common;
using MarketMania.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketMania.DataAccessLayer.Configurations;

internal class SupplierConfiguration : BaseEntityConfiguration<Supplier>
{
    public override void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.Property(s => s.Name).HasMaxLength(255).IsRequired();
        builder.Property(s => s.City).HasMaxLength(50).IsRequired();

        builder.ToTable("Suppliers");
        base.Configure(builder);
    }
}