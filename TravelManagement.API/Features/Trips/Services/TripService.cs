using TravelManagement.API.Features.Trips.DTOs;
using TravelManagement.API.Features.Trips.Interfaces;

namespace TravelManagement.API.Features.Trips.Services;

public class TripService : ITripService
{
    private readonly ITripRepository _repository;

    public TripService(ITripRepository repository)
    {
        _repository = repository;
    }
    
 
    public async Task<IEnumerable<TripDto>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }
    
    public async Task BulkCreateAsync(BulkCreateTripDto dto)
{
    await _repository.BulkCreateAsync(dto);
}

public async Task<TripSearchResultDto> SearchAsync(
    Guid? cityId,
    Guid? projectId,
    Guid? personId,
    string? search)
{
    return await _repository.SearchAsync(
        cityId,
        projectId,
        personId,
        search);
}
    public async Task<TripDto?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<TripDto> CreateAsync(CreateTripDto dto)
    {
        return await _repository.CreateAsync(dto);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateTripDto dto)
    {
        return await _repository.UpdateAsync(id, dto);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }
}