using TravelManagement.API.Features.Users.DTOs;

namespace TravelManagement.API.Features.Users.Interfaces;

public interface IUserService
{
    Task<List<UserResponse>> GetAllAsync();

    Task<UserResponse?> GetByIdAsync(Guid id);

    Task<UserResponse> CreateAsync(CreateUserRequest request);

    Task<UserResponse?> UpdateAsync(Guid id, UpdateUserRequest request);

    Task<bool> DeleteAsync(Guid id);
}