using System.ComponentModel.DataAnnotations;

namespace TravelManagement.API.Features.Users.DTOs;

public class ChangePasswordRequest
{
    [Required]
    [MinLength(6)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;
}