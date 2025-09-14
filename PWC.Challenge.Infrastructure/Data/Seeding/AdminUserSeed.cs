using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Infrastructure.Data.Seeding;

public static class AdminUserSeed
{
    public static void SeedAdminUser(this ModelBuilder modelBuilder)
    {
        var adminUserId = Guid.Parse("a0b0c0d0-e0f0-0000-0000-000000000001");
        var adminRoleId = Guid.Parse("c0f0a0b0-c0d0-e0f0-0000-000000000001"); // From RoleSeed.cs

        var hasher = new PasswordHasher<ApplicationUser>();

        var adminUser = new ApplicationUser
        {
            Id = adminUserId,
            UserName = "admin@pwc.challenge.com",
            NormalizedUserName = "ADMIN@PWC.CHALLENGE.COM",
            Email = "admin@pwc.challenge.com",
            NormalizedEmail = "ADMIN@PWC.CHALLENGE.COM",
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "User"
        };

        adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");

        modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

        modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
            new IdentityUserRole<Guid>
            {
                UserId = adminUserId,
                RoleId = adminRoleId
            }
        );
    }
}
