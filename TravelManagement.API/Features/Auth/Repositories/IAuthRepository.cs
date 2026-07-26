using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Features.Auth.Repositories;

public interface IAuthRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByIdAsync(Guid id);
}