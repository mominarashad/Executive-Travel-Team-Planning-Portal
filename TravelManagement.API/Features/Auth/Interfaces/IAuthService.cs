using TravelManagement.API.Features.Auth.DTOs;

namespace TravelManagement.API.Features.Auth.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<UserDto?> GetCurrentUserAsync(Guid userId);

    Task LogoutAsync();
}