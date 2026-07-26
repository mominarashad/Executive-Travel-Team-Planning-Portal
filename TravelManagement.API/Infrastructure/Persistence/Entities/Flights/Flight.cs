using TravelManagement.API.Common;

namespace TravelManagement.API.Infrastructure.Persistence.Entities.Flights;
public class Flight : BaseEntity
{
    public Guid TripId { get; set; }
    public Trip Trip { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Airline { get; set; } = string.Empty;

    public string FlightNumber { get; set; } = string.Empty;

    public DateTime DepartureTime { get; set; }

    public DateTime ArrivalTime { get; set; }

    public string DepartureAirport { get; set; } = string.Empty;

    public string ArrivalAirport { get; set; } = string.Empty;

    public string Aircraft { get; set; } = string.Empty;

    public string BookingReference { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}