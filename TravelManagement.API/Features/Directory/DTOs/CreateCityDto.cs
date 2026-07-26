using System.ComponentModel.DataAnnotations;

namespace TravelManagement.API.Features.Directory.DTOs;

public class CreateCityDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = string.Empty;
}