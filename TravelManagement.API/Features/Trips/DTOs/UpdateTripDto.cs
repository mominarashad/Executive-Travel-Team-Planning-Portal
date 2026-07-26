using System.ComponentModel.DataAnnotations;

namespace TravelManagement.API.Features.Trips.DTOs;

public class UpdateTripDto
{
    [Required]
    public Guid DestinationCityId { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }

    public Guid? ProjectId { get; set; }

    public Guid? BusinessEntityId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Hotel { get; set; } = string.Empty;

    public string Transport { get; set; } = string.Empty;

    public string FlightInfo { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public List<Guid> TeamMemberIds { get; set; } = new();
}