using TravelManagement.API.Features.OnePager.DTOs;

namespace TravelManagement.API.Features.OnePager.Interfaces;

public interface IOnePagerRepository
{
    Task<OnePagerDto?> GetOnePagerAsync(Guid userId);
}