using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Features.Flights.DTOs;
using TravelManagement.API.Features.Flights.Interfaces;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities.Flights;

namespace TravelManagement.API.Features.Flights.Repositories;

public class FlightRepository : IFlightRepository
{
    private readonly ApplicationDbContext _context;

    public FlightRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // ---------- shared validation ----------

    private async Task ValidateReferencesAsync(Guid tripId, Guid userId)
    {
        var tripExists = await _context.Trips.AnyAsync(t => t.Id == tripId && t.IsActive);
        if (!tripExists)
            throw new InvalidOperationException("Trip not found.");

        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
            throw new InvalidOperationException("Traveller not found.");
    }

    private async Task EnsureNoOverlapAsync(
        Guid userId,
        DateTime departureTime,
        DateTime arrivalTime,
        Guid? excludeFlightId)
    {
        var conflict = await _context.Flights
            .Where(f => f.IsActive
                && f.UserId == userId
                && (excludeFlightId == null || f.Id != excludeFlightId)
                && f.DepartureTime < arrivalTime
                && departureTime < f.ArrivalTime)
            .Select(f => new { f.Airline, f.FlightNumber, f.DepartureTime, f.ArrivalTime })
            .FirstOrDefaultAsync();

        if (conflict != null)
            throw new InvalidOperationException(
                $"Traveller already has a flight ({conflict.Airline} {conflict.FlightNumber}, " +
                $"{conflict.DepartureTime:yyyy-MM-dd HH:mm} to {conflict.ArrivalTime:yyyy-MM-dd HH:mm}) " +
                "that overlaps this time window.");
    }

    private async Task ValidateAndCheckAsync(
        Guid tripId,
        Guid userId,
        DateTime departureTime,
        DateTime arrivalTime,
        Guid? excludeFlightId)
    {
        if (arrivalTime <= departureTime)
            throw new InvalidOperationException("Arrival time must be after departure time.");

        await ValidateReferencesAsync(tripId, userId);
        await EnsureNoOverlapAsync(userId, departureTime, arrivalTime, excludeFlightId);
    }

    // ---------- reads (unchanged) ----------

    public async Task<IEnumerable<FlightDto>> GetAllAsync()
    {
        return await _context.Flights
            .Where(f => f.IsActive)
            .OrderBy(f => f.DepartureTime)
            .Select(f => new FlightDto
            {
                Id = f.Id,
                TripId = f.TripId,
                UserId = f.UserId,
                TravellerName = f.User.Name,
                Airline = f.Airline,
                FlightNumber = f.FlightNumber,
                DepartureTime = f.DepartureTime,
                ArrivalTime = f.ArrivalTime,
                DepartureAirport = f.DepartureAirport,
                ArrivalAirport = f.ArrivalAirport,
                Aircraft = f.Aircraft,
                BookingReference = f.BookingReference,
                GoogleFlightsUrl = BuildGoogleFlightsUrl(f)
            })
            .ToListAsync();
    }

    public async Task<FlightDto?> GetByIdAsync(Guid id)
    {
        return await _context.Flights
            .Where(f => f.Id == id && f.IsActive)
            .Select(f => new FlightDto
            {
                Id = f.Id,
                TripId = f.TripId,
                UserId = f.UserId,
                TravellerName = f.User.Name,
                Airline = f.Airline,
                FlightNumber = f.FlightNumber,
                DepartureTime = f.DepartureTime,
                ArrivalTime = f.ArrivalTime,
                DepartureAirport = f.DepartureAirport,
                ArrivalAirport = f.ArrivalAirport,
                Aircraft = f.Aircraft,
                BookingReference = f.BookingReference,
                GoogleFlightsUrl = BuildGoogleFlightsUrl(f)
            })
            .FirstOrDefaultAsync();
    }

    // ---------- writes (validated) ----------

    public async Task<FlightDto> CreateAsync(CreateFlightDto dto)
    {
        await ValidateAndCheckAsync(dto.TripId, dto.UserId, dto.DepartureTime, dto.ArrivalTime, excludeFlightId: null);

        var flight = new Flight
        {
            Id = Guid.NewGuid(),
            TripId = dto.TripId,
            UserId = dto.UserId,
            Airline = dto.Airline,
            FlightNumber = dto.FlightNumber,
            DepartureTime = dto.DepartureTime,
            ArrivalTime = dto.ArrivalTime,
            DepartureAirport = dto.DepartureAirport,
            ArrivalAirport = dto.ArrivalAirport,
            Aircraft = dto.Aircraft,
            BookingReference = dto.BookingReference,
            IsActive = true
        };

        _context.Flights.Add(flight);
        await _context.SaveChangesAsync();

        return new FlightDto
        {
            Id = flight.Id,
            TripId = flight.TripId,
            UserId = flight.UserId,
            TravellerName = (await _context.Users.FindAsync(flight.UserId))?.Name ?? string.Empty,
            Airline = flight.Airline,
            FlightNumber = flight.FlightNumber,
            DepartureTime = flight.DepartureTime,
            ArrivalTime = flight.ArrivalTime,
            DepartureAirport = flight.DepartureAirport,
            ArrivalAirport = flight.ArrivalAirport,
            Aircraft = flight.Aircraft,
            BookingReference = flight.BookingReference,
            GoogleFlightsUrl = BuildGoogleFlightsUrl(flight)
        };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateFlightDto dto)
    {
        var flight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == id && f.IsActive);

        if (flight == null)
            return false;

        await ValidateAndCheckAsync(dto.TripId, dto.UserId, dto.DepartureTime, dto.ArrivalTime, excludeFlightId: flight.Id);

        flight.TripId = dto.TripId;
        flight.UserId = dto.UserId;
        flight.Airline = dto.Airline;
        flight.FlightNumber = dto.FlightNumber;
        flight.DepartureTime = dto.DepartureTime;
        flight.ArrivalTime = dto.ArrivalTime;
        flight.DepartureAirport = dto.DepartureAirport;
        flight.ArrivalAirport = dto.ArrivalAirport;
        flight.Aircraft = dto.Aircraft;
        flight.BookingReference = dto.BookingReference;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var flight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == id && f.IsActive);

        if (flight == null)
            return false;

        flight.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    private static string BuildGoogleFlightsUrl(Flight flight)
    {
        return $"https://www.google.com/travel/flights?hl=en#flt={flight.DepartureAirport}.{flight.ArrivalAirport}.{flight.DepartureTime:yyyy-MM-dd}";
    }
}
