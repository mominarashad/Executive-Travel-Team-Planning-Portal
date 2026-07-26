using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities.Flights;

namespace TravelManagement.API.Infrastructure.Seed;

public static class FlightSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Flights.AnyAsync())
            return;

        var dubaiTrip = await context.Trips
            .Include(t => t.Project)
            .FirstAsync(t => t.Project!.Name == "AI Expansion");

        var lahoreTrip = await context.Trips
            .Include(t => t.Project)
            .FirstAsync(t => t.Project!.Name == "Client Visit");

        var karachiTrip = await context.Trips
            .Include(t => t.Project)
            .FirstAsync(t => t.Project!.Name == "Partnership");

        context.Flights.AddRange(

            new Flight
            {
                Id = Guid.NewGuid(),
                TripId = dubaiTrip.Id,
                UserId = Guid.Parse("52b920f5-e6e8-409b-b735-b609bef03f5c"),

                Airline = "Emirates",
                Aircraft = "Boeing 777-300ER",
                FlightNumber = "EK625",
                DepartureAirport = "LHE",
                ArrivalAirport = "DXB",
                DepartureTime = new DateTime(2026, 8, 10, 9, 30, 0, DateTimeKind.Utc),
                ArrivalTime = new DateTime(2026, 8, 10, 12, 10, 0, DateTimeKind.Utc),
                BookingReference = "EMR12345",
                IsActive = true
            },

            new Flight
            {
                Id = Guid.NewGuid(),
                TripId = lahoreTrip.Id,
                UserId = Guid.Parse("4c4e8ed4-09e4-4e46-86f1-c51cd96bc1a2"),

                Airline = "Road Travel",
                Aircraft = "Toyota Land Cruiser",
                FlightNumber = "CAR-001",
                DepartureAirport = "LHE",
                ArrivalAirport = "LHE",
                DepartureTime = new DateTime(2026, 9, 5, 8, 0, 0, DateTimeKind.Utc),
                ArrivalTime = new DateTime(2026, 9, 5, 8, 0, 0, DateTimeKind.Utc),
                BookingReference = "LOCAL001",
                IsActive = true
            },

            new Flight
            {
                Id = Guid.NewGuid(),
                TripId = karachiTrip.Id,
                UserId = Guid.Parse("4af19fec-ef0d-4da4-a0ac-d59c7080752f"),

                Airline = "PIA",
                Aircraft = "Airbus A320",
                FlightNumber = "PK305",
                DepartureAirport = "LHE",
                ArrivalAirport = "KHI",
                DepartureTime = new DateTime(2026, 10, 1, 7, 30, 0, DateTimeKind.Utc),
                ArrivalTime = new DateTime(2026, 10, 1, 9, 20, 0, DateTimeKind.Utc),
                BookingReference = "PIA56789",
                IsActive = true
            }

        );

        await context.SaveChangesAsync();
    }
}