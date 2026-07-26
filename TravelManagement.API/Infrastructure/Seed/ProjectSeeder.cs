using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Seed;

public static class ProjectSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Projects.AnyAsync())
            return;

        context.Projects.AddRange(

            new Project
            {
                Id = Guid.NewGuid(),
                Name = "AI Expansion",
                IsSystem = true,
                IsActive = true
            },

            new Project
            {
                Id = Guid.NewGuid(),
                Name = "Client Visit",
                IsSystem = true,
                IsActive = true
            },

            new Project
            {
                Id = Guid.NewGuid(),
                Name = "Recruitment",
                IsSystem = true,
                IsActive = true
            },

            new Project
            {
                Id = Guid.NewGuid(),
                Name = "Partnership",
                IsSystem = true,
                IsActive = true
            }

        );

        await context.SaveChangesAsync();
    }
}