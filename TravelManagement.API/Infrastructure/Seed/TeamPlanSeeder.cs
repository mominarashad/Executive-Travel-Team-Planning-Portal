using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Seed;

public static class TeamPlanSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.TeamPlanEntries.AnyAsync())
            return;

        var alex = await context.Users.FirstAsync(u => u.Name == "Alex Morgan");
        var sarah = await context.Users.FirstAsync(u => u.Name == "Sarah Ahmed");
        var john = await context.Users.FirstAsync(u => u.Name == "John Williams");
        var maria = await context.Users.FirstAsync(u => u.Name == "Maria Garcia");

        var dubai = await context.Cities.FirstAsync(c => c.Name == "Dubai");
        var lahore = await context.Cities.FirstAsync(c => c.Name == "Lahore");
        var karachi = await context.Cities.FirstAsync(c => c.Name == "Karachi");

        context.TeamPlanEntries.AddRange(

            new TeamPlanEntry
            {
                Id = Guid.NewGuid(),
                UserId = alex.Id,
                CityId = dubai.Id,
                FromDate = new DateOnly(2026, 8, 10),
                ToDate = new DateOnly(2026, 8, 15),
                Type = "Trip",
                ApprovalStatus = "Approved",
                Notes = "Executive meetings",
                IsActive = true
            },

            new TeamPlanEntry
            {
                Id = Guid.NewGuid(),
                UserId = sarah.Id,
                CityId = lahore.Id,
                FromDate = new DateOnly(2026, 9, 5),
                ToDate = new DateOnly(2026, 9, 7),
                Type = "Trip",
                ApprovalStatus = "Approved",
                Notes = "Client visit",
                IsActive = true
            },

            new TeamPlanEntry
            {
                Id = Guid.NewGuid(),
                UserId = john.Id,
                CityId = null,
                FromDate = new DateOnly(2026, 9, 20),
                ToDate = new DateOnly(2026, 9, 25),
                Type = "Vacation",
                ApprovalStatus = "Pending",
                Notes = "Annual leave",
                IsActive = true
            },

            new TeamPlanEntry
            {
                Id = Guid.NewGuid(),
                UserId = maria.Id,
                CityId = karachi.Id,
                FromDate = new DateOnly(2026, 10, 1),
                ToDate = new DateOnly(2026, 10, 4),
                Type = "Remote",
                ApprovalStatus = "Approved",
                Notes = "Working remotely",
                IsActive = true
            }

        );

        await context.SaveChangesAsync();
    }
}