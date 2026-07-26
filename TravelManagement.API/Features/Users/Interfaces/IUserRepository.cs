using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Features.Users.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();

    Task<User?> GetByIdAsync(Guid id);

    Task<User?> GetByEmailAsync(string email);

    Task AddAsync(User user);

    Task UpdateAsync(User user);

    Task DeleteAsync(User user);

    Task SaveChangesAsync();
}