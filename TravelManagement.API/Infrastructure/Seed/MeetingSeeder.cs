using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Seed;

public static class MeetingSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Meetings.AnyAsync())
            return;

        // Trips
        var dubaiTrip = await context.Trips
            .Include(t => t.DestinationCity)
            .Include(t => t.Project)
            .Include(t => t.BusinessEntity)
            .FirstAsync(t => t.DestinationCity.Name == "Dubai");

        var lahoreTrip = await context.Trips
            .Include(t => t.DestinationCity)
            .Include(t => t.Project)
            .Include(t => t.BusinessEntity)
            .FirstAsync(t => t.DestinationCity.Name == "Lahore");

        var karachiTrip = await context.Trips
            .Include(t => t.DestinationCity)
            .Include(t => t.Project)
            .Include(t => t.BusinessEntity)
            .FirstAsync(t => t.DestinationCity.Name == "Karachi");

        // Contacts
        var john = await context.Contacts.FirstAsync(c => c.Name == "John Smith");
        var ahmed = await context.Contacts.FirstAsync(c => c.Name == "Ahmed Khan");
        var sara = await context.Contacts.FirstAsync(c => c.Name == "Sara Ali");
        var ali = await context.Contacts.FirstAsync(c => c.Name == "Ali Raza");

        context.Meetings.AddRange(

            new Meeting
            {
                Id = Guid.NewGuid(),
                TripId = dubaiTrip.Id,
                ContactId = john.Id,
                DisplayOrder = 1,
                Priority = "High",
                Status = "Confirmed",
                ScheduledTime = new TimeOnly(9, 30),
                ProjectId = dubaiTrip.ProjectId,
                BusinessEntityId = dubaiTrip.BusinessEntityId,
                Agenda = "Discuss expansion strategy",
                IsActive = true
            },

            new Meeting
            {
                Id = Guid.NewGuid(),
                TripId = lahoreTrip.Id,
                ContactId = ahmed.Id,
                DisplayOrder = 1,
                Priority = "High",
                Status = "Confirmed",
                ScheduledTime = new TimeOnly(10, 00),
                ProjectId = lahoreTrip.ProjectId,
                BusinessEntityId = lahoreTrip.BusinessEntityId,
                Agenda = "Executive discussion",
                IsActive = true
            },

            new Meeting
            {
                Id = Guid.NewGuid(),
                TripId = lahoreTrip.Id,
                ContactId = sara.Id,
                DisplayOrder = 2,
                Priority = "Medium",
                Status = "Proposed",
                ScheduledTime = new TimeOnly(14, 00),
                ProjectId = lahoreTrip.ProjectId,
                BusinessEntityId = lahoreTrip.BusinessEntityId,
                Agenda = "Hiring collaboration",
                IsActive = true
            },

            new Meeting
            {
                Id = Guid.NewGuid(),
                TripId = karachiTrip.Id,
                ContactId = ali.Id,
                DisplayOrder = 1,
                Priority = "High",
                Status = "Completed",
                ScheduledTime = new TimeOnly(11, 30),
                ProjectId = karachiTrip.ProjectId,
                BusinessEntityId = karachiTrip.BusinessEntityId,
                Agenda = "Technology partnership",
                IsActive = true
            }

        );

        await context.SaveChangesAsync();
    }
}