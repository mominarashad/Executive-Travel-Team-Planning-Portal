using System.ComponentModel.DataAnnotations;

namespace TravelManagement.API.Features.Meetings.DTOs;

public class CreateMeetingDto
{
    [Required]
    public Guid TripId { get; set; }

    [Required]
    public Guid ContactId { get; set; }

    public int DisplayOrder { get; set; }

    public string Priority { get; set; } = "Medium";

    public string Status { get; set; } = "Proposed";

    public TimeOnly? ScheduledTime { get; set; }

    public Guid? ProjectId { get; set; }

public Guid? BusinessEntityId { get; set; }
    public string Agenda { get; set; } = string.Empty;

    public List<Guid> AttendeeIds { get; set; } = new();

    

    public List<CreateMeetingMaterialDto> Materials { get; set; } = new();
}