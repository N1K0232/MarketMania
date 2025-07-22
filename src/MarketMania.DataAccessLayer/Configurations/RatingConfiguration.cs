using MarketMania.DataAccessLayer.Configurations.Common;
using MarketMania.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketMania.DataAccessLayer.Configurations;

internal class RatingConfiguration : BaseEntityConfiguration<Rating>
{
    public override void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.Property(r => r.Title).HasMaxLength(255).IsRequired();
        builder.Property(r => r.Text).HasColumnType("NVARCHAR(MAX)").IsRequired();

        builder.HasOne(r => r.Product)
            .WithMany(p => p.Ratings)
            .HasForeignKey(r => r.ProductId)
            .HasConstraintName("FK_Ratings_Products")
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Ratings");
        base.Configure(builder);
    }
}