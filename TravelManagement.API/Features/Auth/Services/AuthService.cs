using BCrypt.Net;
using TravelManagement.API.Features.Auth.DTOs;
using TravelManagement.API.Features.Auth.Interfaces;
using TravelManagement.API.Features.Auth.Repositories;
using TravelManagement.API.Infrastructure.Identity;
namespace TravelManagement.API.Features.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
    IAuthRepository repository,
    IJwtTokenGenerator jwtTokenGenerator)
    {
        _repository = repository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _repository.GetUserByEmailAsync(request.Email);
        if (user is null)
            return null;

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash);

        if (!isPasswordValid)
            return null;

        var token = _jwtTokenGenerator.GenerateToken(
            user,
            user.Role.Name);

        return new LoginResponse
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.Name,
                IsCeo = user.IsCeo
            }
        };
    }
    public async Task<UserDto?> GetCurrentUserAsync(Guid userId)
{
    var user = await _repository.GetUserByIdAsync(userId);

    if (user is null)
        return null;

    return new UserDto
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        Role = user.Role.Name,
        IsCeo = user.IsCeo
    };
}
public Task LogoutAsync()
{
    return Task.CompletedTask;
}
}