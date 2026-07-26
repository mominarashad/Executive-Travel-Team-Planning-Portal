namespace TravelManagement.API.Features.OnePager.DTOs;

public class OnePagerMeetingDto
{
    public Guid TripId { get; set; }
    public string TripCity { get; set; } = string.Empty;
    public DateOnly TripStartDate { get; set; }
    public DateOnly TripEndDate { get; set; }

    public int DisplayOrder { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string BusinessEntityName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public TimeOnly? ScheduledTime { get; set; }
    public string Agenda { get; set; } = string.Empty;

    public List<string> Team { get; set; } = new();
    public List<OnePagerMaterialDto> Materials { get; set; } = new();
}