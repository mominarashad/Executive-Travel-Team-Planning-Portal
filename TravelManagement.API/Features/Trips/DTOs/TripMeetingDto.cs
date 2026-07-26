namespace TravelManagement.API.Features.Trips.DTOs;

public class TripMeetingDto
{
    public Guid Id { get; set; }

    public string ContactName { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public string Priority { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public TimeOnly? ScheduledTime { get; set; }

    public Guid? ProjectId { get; set; }

    public string? ProjectName { get; set; }

    public Guid? BusinessEntityId { get; set; }

    public string? BusinessEntityName { get; set; }

    public string Agenda { get; set; } = string.Empty;

    public List<TripMeetingAttendeeDto> Attendees { get; set; } = new();

    public List<TripMeetingMaterialDto> Materials { get; set; } = new();
}