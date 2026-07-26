namespace TravelManagement.API.Features.Directory.DTOs;

public class CityDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;
}