using BCrypt.Net;
using TravelManagement.API.Features.Users.DTOs;
using TravelManagement.API.Features.Users.Interfaces;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Features.Users.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<UserResponse>> GetAllAsync()
    {
        var users = await _repository.GetAllAsync();

        return users.Select(u => new UserResponse
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Title = u.Title,
            Function = u.Function,
            IsCeo = u.IsCeo,
            Role = u.Role.Name
        }).ToList();
    }

    public async Task<UserResponse?> GetByIdAsync(Guid id)
    {
        var user = await _repository.GetByIdAsync(id);

        if (user == null)
            return null;

        return MapToResponse(user);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request)
    {
        var existingUser = await _repository.GetByEmailAsync(request.Email);

        if (existingUser != null)
            throw new Exception("Email already exists.");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Title = request.Title,
            Function = request.Function,
            RoleId = request.RoleId,
            IsCeo = request.IsCeo
        };

        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        var created = await _repository.GetByIdAsync(user.Id);

        return MapToResponse(created!);
    }

    public async Task<UserResponse?> UpdateAsync(Guid id, UpdateUserRequest request)
{
    var user = await _repository.GetByIdAsync(id);

    if (user == null)
        return null;

    user.Name = request.Name;
    user.Email = request.Email;
    user.Title = request.Title;
    user.Function = request.Function;
    user.RoleId = request.RoleId;
    user.IsCeo = request.IsCeo;

    await _repository.UpdateAsync(user);
    await _repository.SaveChangesAsync();

    return MapToResponse(user);
}

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _repository.GetByIdAsync(id);

        if (user == null)
            return false;

        await _repository.DeleteAsync(user);
        await _repository.SaveChangesAsync();

        return true;
    }

    private static UserResponse MapToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Title = user.Title,
            Function = user.Function,
            IsCeo = user.IsCeo,
            Role = user.Role.Name
        };
    }
}