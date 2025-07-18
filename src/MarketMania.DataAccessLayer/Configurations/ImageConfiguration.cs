using MarketMania.DataAccessLayer.Configurations.Common;
using MarketMania.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketMania.DataAccessLayer.Configurations;

internal class ImageConfiguration : BaseEntityConfiguration<Image>
{
    public override void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.Property(i => i.Path).HasMaxLength(255).IsRequired();
        builder.Property(i => i.ContentType).HasMaxLength(100).IsRequired();

        builder.Property(i => i.IsPublished).HasDefaultValueSql("((1))");

        builder.HasOne(i => i.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(i => i.ProductId)
            .HasConstraintName("FK_Images_Products");

        builder.HasIndex(i => i.Path)
            .IsClustered(false)
            .IsUnique(false)
            .HasDatabaseName("IX_Images_Path");

        builder.HasIndex(i => new { i.ProductId, i.Path })
            .IsClustered(false)
            .IsUnique()
            .HasDatabaseName("IX_Images_UniquePath");

        builder.ToTable("Images");
        base.Configure(builder);
    }
}