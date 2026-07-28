namespace TravelManagement.API.Features.OnePager.DTOs;

public class OnePagerFlightDto
{
    public string TripCity { get; set; } = string.Empty;
    public string Airline { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string DepartureAirport { get; set; } = string.Empty;
    public string ArrivalAirport { get; set; } = string.Empty;
    public string Aircraft { get; set; } = string.Empty;
    public string BookingReference { get; set; } = string.Empty;
}

public class OnePagerDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Function { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }

    public List<OnePagerItineraryEntryDto> Itinerary { get; set; } = new();
    public List<DaysByCountryDto> DaysByCountry { get; set; } = new();
    public int TotalDays { get; set; }

    public List<OnePagerFlightDto> Flights { get; set; } = new();
    public List<OnePagerMeetingDto> Meetings { get; set; } = new();
}