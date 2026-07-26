using TravelManagement.API.Features.Directory.DTOs;
using TravelManagement.API.Features.Directory.Interfaces;

namespace TravelManagement.API.Features.Directory.Services;

public class CityService : ICityService
{
    private readonly ICityRepository _repository;

    public CityService(ICityRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CityDto>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<CityDto?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<CityDto> CreateAsync(CreateCityDto dto)
    {
        return await _repository.CreateAsync(dto);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateCityDto dto)
    {
        return await _repository.UpdateAsync(id, dto);
    }

    public async Task<IEnumerable<CityDto>> AutocompleteAsync(string term)
    {
        return await _repository.AutocompleteAsync(term);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }
}