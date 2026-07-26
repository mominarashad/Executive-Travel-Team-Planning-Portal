namespace TravelManagement.API.Features.Meetings.DTOs;

public class MeetingMaterialDto
{
    public Guid Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public Guid? OwnerId { get; set; }

    public string? OwnerName { get; set; }
}