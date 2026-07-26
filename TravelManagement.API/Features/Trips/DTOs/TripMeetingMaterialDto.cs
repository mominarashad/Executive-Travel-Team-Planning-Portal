namespace TravelManagement.API.Features.Trips.DTOs;

public class TripMeetingMaterialDto
{
    public Guid Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public string? OwnerName { get; set; }
}