using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Seed;

public static class RoleSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Roles.AnyAsync())
            return;

        context.Roles.AddRange(

            new Role
            {
                Id = Guid.NewGuid(),
                Name = "Admin"
            },

            new Role
            {
                Id = Guid.NewGuid(),
                Name = "Employee"
            }

        );

        await context.SaveChangesAsync();
    }
}