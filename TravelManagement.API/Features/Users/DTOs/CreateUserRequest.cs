using System.ComponentModel.DataAnnotations;

namespace TravelManagement.API.Features.Users.DTOs;

public class CreateUserRequest
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Function { get; set; } = string.Empty;

    public bool IsCeo { get; set; }

    [Required]
    public Guid RoleId { get; set; }
}