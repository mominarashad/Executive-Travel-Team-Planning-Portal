using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Infrastructure.Identity;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, string role);
}