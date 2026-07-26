using TravelManagement.API.Features.OnePager.DTOs;

namespace TravelManagement.API.Features.OnePager.Interfaces;

public interface IOnePagerService
{
    Task<OnePagerDto?> GetOnePagerAsync(Guid userId);
}