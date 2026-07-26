using System.ComponentModel.DataAnnotations;

namespace TravelManagement.API.Features.Hotels.DTOs;

public class CreateHotelDto
{
    [Required]
    public Guid CityId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public bool IsCustom { get; set; } = true;
}