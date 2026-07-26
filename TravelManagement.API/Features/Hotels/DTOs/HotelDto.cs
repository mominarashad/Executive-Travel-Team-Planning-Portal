namespace TravelManagement.API.Features.Hotels.DTOs;

public class HotelDto
{
    public Guid Id { get; set; }
    public Guid CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsCustom { get; set; }
}