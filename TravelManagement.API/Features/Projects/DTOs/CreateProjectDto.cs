using System.ComponentModel.DataAnnotations;

namespace TravelManagement.API.Features.Projects.DTOs;

public class CreateProjectDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
}