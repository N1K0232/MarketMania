using MarketMania.Authentication.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MarketMania.Authentication;

public abstract class AuthenticationDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, IdentityUserClaim<Guid>, ApplicationUserRole,
    IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>(options), IDataProtectionKeyContext
{
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    public DbSet<Subscription> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(b =>
        {
            b.Property(u => u.FirstName).HasMaxLength(256).IsRequired();
            b.Property(u => u.LastName).HasMaxLength(256).IsRequired(false);
        });

        builder.Entity<ApplicationUserRole>(b =>
        {
            b.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            b.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
        });

        builder.Entity<DataProtectionKey>(b =>
        {
            b.HasKey(k => k.Id);
            b.Property(k => k.Id).ValueGeneratedOnAdd();

            b.Property(k => k.FriendlyName).HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            b.Property(k => k.Xml).HasColumnType("NVARCHAR(MAX)").IsRequired(false);
        });

        builder.Entity<Subscription>(b =>
        {
            b.ToTable("Subscriptions");
            b.HasKey(s => s.Id);
            b.Property(s => s.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

            b.Property(s => s.UserName).HasMaxLength(255).IsRequired();
            b.Property(s => s.ApiKey).HasMaxLength(512).IsRequired();

            b.HasIndex(s => s.UserName, "IX_Subscriptions").IsUnique();
            b.HasIndex(s => s.ApiKey, "IX_Subscriptions_ApiKey").IsUnique();
        });
    }
}