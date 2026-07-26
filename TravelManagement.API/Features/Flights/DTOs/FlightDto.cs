namespace TravelManagement.API.Features.Flights.DTOs;

public class FlightDto
{
    public Guid Id { get; set; }

    public Guid TripId { get; set; }

    public Guid UserId { get; set; }

    public string TravellerName { get; set; } = string.Empty;

    public string Airline { get; set; } = string.Empty;

    public string FlightNumber { get; set; } = string.Empty;

    public DateTime DepartureTime { get; set; }

    public DateTime ArrivalTime { get; set; }

    public string DepartureAirport { get; set; } = string.Empty;

    public string ArrivalAirport { get; set; } = string.Empty;

    public string Aircraft { get; set; } = string.Empty;

    public string BookingReference { get; set; } = string.Empty;

    public string GoogleFlightsUrl { get; set; } = string.Empty;
}