namespace TravelManagement.API.Features.Trips.DTOs;

public class TripSearchResultDto
{
    public List<TripDto> Upcoming { get; set; } = new();

    public List<TripDto> Past { get; set; } = new();
}