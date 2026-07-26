namespace TravelManagement.API.Features.Meetings.DTOs;

public class CreateMeetingMaterialDto
{
    public string Description { get; set; } = string.Empty;

    public Guid? OwnerId { get; set; }
}