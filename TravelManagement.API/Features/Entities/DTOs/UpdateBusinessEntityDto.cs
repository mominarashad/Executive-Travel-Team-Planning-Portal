using System.ComponentModel.DataAnnotations;

namespace TravelManagement.API.Features.Entities.DTOs;

public class UpdateBusinessEntityDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
}