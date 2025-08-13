using MarketMania.DataAccessLayer.Configurations.Common;
using MarketMania.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketMania.DataAccessLayer.Configurations;

internal class SpecificationConfiguration : BaseEntityConfiguration<Specification>
{
    public override void Configure(EntityTypeBuilder<Specification> builder)
    {
        builder.Property(s => s.Name).HasMaxLength(256).IsRequired();
        builder.Property(s => s.Description).HasColumnType("NVARCHAR(MAX)").IsRequired();

        builder.HasOne(s => s.Product)
            .WithMany(p => p.Specifications)
            .HasForeignKey(s => s.ProductId)
            .HasConstraintName("FK_Specifications_Products")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasIndex(s => s.Name)
            .IsClustered(false)
            .IsUnique(false)
            .HasDatabaseName("IX_Specifications_Name");

        builder.HasIndex(s => new { s.ProductId, s.Name })
            .IsClustered(false)
            .IsUnique()
            .HasDatabaseName("IX_Specifications_UniqueName");

        builder.ToTable("Specifications");
        base.Configure(builder);
    }
}