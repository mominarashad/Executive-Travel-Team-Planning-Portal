namespace TravelManagement.API.Features.OnePager.DTOs;

public class OnePagerItineraryEntryDto
{
    public string Source { get; set; } = string.Empty;  // "Trip" | "TeamPlan"
    public string Type { get; set; } = string.Empty;
    public Guid? CityId { get; set; }
    public string CityName { get; set; } = "TBC";
    public string Country { get; set; } = string.Empty;
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public Guid? TripId { get; set; }
    public string? Notes { get; set; }
}