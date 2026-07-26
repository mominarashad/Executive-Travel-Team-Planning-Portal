using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Seed;

public static class TripMemberSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.TripMembers.AnyAsync())
            return;

        var admin = await context.Users
            .FirstAsync(u => u.Email == "admin@travelmanagement.com");

        var dubaiTrip = await context.Trips
            .Include(t => t.DestinationCity)
            .FirstAsync(t => t.DestinationCity.Name == "Dubai");

        var lahoreTrip = await context.Trips
            .Include(t => t.DestinationCity)
            .FirstAsync(t => t.DestinationCity.Name == "Lahore");

        context.TripMembers.AddRange(

            new TripMember
            {
                Id = Guid.NewGuid(),
                TripId = dubaiTrip.Id,
                UserId = admin.Id
            },

            new TripMember
            {
                Id = Guid.NewGuid(),
                TripId = lahoreTrip.Id,
                UserId = admin.Id
            }

        );

        await context.SaveChangesAsync();
    }
}