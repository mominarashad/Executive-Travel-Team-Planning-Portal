namespace TravelManagement.API.Features.Trips.DTOs;

public class TripTeamMemberDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
}