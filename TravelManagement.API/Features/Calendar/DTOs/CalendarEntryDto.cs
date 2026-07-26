namespace TravelManagement.API.Features.Calendar.DTOs;

public class CalendarEntryDto
{
    public string Source { get; set; } = string.Empty;   // "Trip" | "TeamPlan"
    public string Type { get; set; } = string.Empty;      // Trip / Option / Vacation / Remote
    public Guid? CityId { get; set; }
    public string CityName { get; set; } = "TBC";
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public string? ApprovalStatus { get; set; }
    public Guid? TripId { get; set; }
    public string? Notes { get; set; }
}