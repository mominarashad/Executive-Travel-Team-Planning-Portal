using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Seed;

public static class BusinessEntitySeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.BusinessEntities.AnyAsync())
            return;

        context.BusinessEntities.AddRange(

            new BusinessEntity
            {
                Id = Guid.NewGuid(),
                Name = "Travel Partner",
                IsSystem = true,
                IsActive = true
            },

            new BusinessEntity
            {
                Id = Guid.NewGuid(),
                Name = "NETSOL",
                IsSystem = true,
                IsActive = true
            },

            new BusinessEntity
            {
                Id = Guid.NewGuid(),
                Name = "Arbisoft",
                IsSystem = true,
                IsActive = true
            },

            new BusinessEntity
            {
                Id = Guid.NewGuid(),
                Name = "Systems Ltd",
                IsSystem = true,
                IsActive = true
            }

        );

        await context.SaveChangesAsync();
    }
}