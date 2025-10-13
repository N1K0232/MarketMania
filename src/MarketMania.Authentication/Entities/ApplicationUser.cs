using Microsoft.AspNetCore.Identity;

namespace MarketMania.Authentication.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public bool EnableNotifications { get; set; }

    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = [];
}