using System.ComponentModel.DataAnnotations;

namespace TravelManagement.API.Features.Directory.DTOs;

public class UpdateCityDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = string.Empty;
}