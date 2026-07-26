namespace TravelManagement.API.Features.Trips.DTOs;

public class TripDto
{
    public Guid Id { get; set; }

    public Guid DestinationCityId { get; set; }

    public string DestinationCity { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public Guid? ProjectId { get; set; }

    public string? ProjectName { get; set; }

    public Guid? BusinessEntityId { get; set; }

    public string? BusinessEntityName { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Hotel { get; set; } = string.Empty;

    public string Transport { get; set; } = string.Empty;

    public string FlightInfo { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public List<Guid> TeamMemberIds { get; set; } = new();

    public List<TripTeamMemberDto> TeamMembers { get; set; } = new();

    public List<TripMeetingDto> Meetings { get; set; } = new();
}