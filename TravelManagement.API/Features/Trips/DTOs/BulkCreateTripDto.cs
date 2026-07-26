namespace TravelManagement.API.Features.Trips.DTOs;

public class BulkCreateTripDto
{
    public List<CreateTripDto> Trips { get; set; } = new();
}