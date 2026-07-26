using System.ComponentModel.DataAnnotations;

namespace TravelManagement.API.Features.Hotels.DTOs;

public class UpdateHotelDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
}