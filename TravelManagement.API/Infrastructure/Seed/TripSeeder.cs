using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Seed;

public static class TripSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Trips.AnyAsync())
            return;

        var dubai = await context.Cities.FirstAsync(c => c.Name == "Dubai");
        var lahore = await context.Cities.FirstAsync(c => c.Name == "Lahore");
        var karachi = await context.Cities.FirstAsync(c => c.Name == "Karachi");

        var aiProject = await context.Projects.FirstAsync(p => p.Name == "AI Expansion");
        var clientVisit = await context.Projects.FirstAsync(p => p.Name == "Client Visit");
        var partnership = await context.Projects.FirstAsync(p => p.Name == "Partnership");

        var travelPartner = await context.BusinessEntities.FirstAsync(e => e.Name == "Travel Partner");
        var netsol = await context.BusinessEntities.FirstAsync(e => e.Name == "NETSOL");
        var systems = await context.BusinessEntities.FirstAsync(e => e.Name == "Systems Ltd");

        context.Trips.AddRange(

            new Trip
            {
                Id = Guid.NewGuid(),
                DestinationCityId = dubai.Id,
                StartDate = new DateOnly(2026, 8, 10),
                EndDate = new DateOnly(2026, 8, 15),
                ProjectId = aiProject.Id,
                BusinessEntityId = travelPartner.Id,
                Status = "Planned",
                Hotel = "Hilton Dubai",
                Transport = "Emirates",
                FlightInfo = "EK-625",
                Notes = "Executive meetings",
                IsActive = true
            },

            new Trip
            {
                Id = Guid.NewGuid(),
                DestinationCityId = lahore.Id,
                StartDate = new DateOnly(2026, 9, 5),
                EndDate = new DateOnly(2026, 9, 7),
                ProjectId = clientVisit.Id,
                BusinessEntityId = netsol.Id,
                Status = "Confirmed",
                Hotel = "Pearl Continental",
                Transport = "Car",
                FlightInfo = "",
                Notes = "Customer meetings",
                IsActive = true
            },

            new Trip
            {
                Id = Guid.NewGuid(),
                DestinationCityId = karachi.Id,
                StartDate = new DateOnly(2026, 10, 1),
                EndDate = new DateOnly(2026, 10, 4),
                ProjectId = partnership.Id,
                BusinessEntityId = systems.Id,
                Status = "Completed",
                Hotel = "Marriott Karachi",
                Transport = "PIA",
                FlightInfo = "PK-305",
                Notes = "Business partnership",
                IsActive = true
            }

        );

        await context.SaveChangesAsync();
    }
}