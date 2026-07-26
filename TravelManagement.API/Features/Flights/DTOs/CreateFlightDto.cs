using System.ComponentModel.DataAnnotations;

namespace TravelManagement.API.Features.Flights.DTOs;

public class CreateFlightDto
{
    [Required]
    public Guid TripId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Airline { get; set; } = string.Empty;

    [Required]
    public string FlightNumber { get; set; } = string.Empty;

    [Required]
    public DateTime DepartureTime { get; set; }

    [Required]
    public DateTime ArrivalTime { get; set; }

    public string DepartureAirport { get; set; } = string.Empty;

    public string ArrivalAirport { get; set; } = string.Empty;

    public string Aircraft { get; set; } = string.Empty;

    public string BookingReference { get; set; } = string.Empty;
}