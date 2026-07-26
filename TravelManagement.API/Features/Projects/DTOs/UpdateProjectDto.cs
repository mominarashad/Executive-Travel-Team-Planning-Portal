using System.ComponentModel.DataAnnotations;

namespace TravelManagement.API.Features.Projects.DTOs;

public class UpdateProjectDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
}