namespace TravelManagement.API.Features.Users.DTOs;

public class UserResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Function { get; set; } = string.Empty;

    public bool IsCeo { get; set; }

    public string Role { get; set; } = string.Empty;
}