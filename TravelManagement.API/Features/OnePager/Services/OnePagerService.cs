using TravelManagement.API.Features.OnePager.DTOs;
using TravelManagement.API.Features.OnePager.Interfaces;

namespace TravelManagement.API.Features.OnePager.Services;

public class OnePagerService : IOnePagerService
{
    private readonly IOnePagerRepository _repository;

    public OnePagerService(IOnePagerRepository repository)
    {
        _repository = repository;
    }

    public async Task<OnePagerDto?> GetOnePagerAsync(Guid userId) =>
        await _repository.GetOnePagerAsync(userId);
}