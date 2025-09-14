using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PWC.Challenge.Infrastructure.Data.Seeding;

public static class RoleSeed
{
    public static void SeedRoles(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityRole<Guid>>().HasData(
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("c0f0a0b0-c0d0-e0f0-0000-000000000001"),
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("c0f0a0b0-c0d0-e0f0-0000-000000000002"),
                Name = "Customer",
                NormalizedName = "CUSTOMER"
            }
        );
    }
}
