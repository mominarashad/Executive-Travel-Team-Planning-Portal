using TravelManagement.API.Features.Directory.DTOs;

namespace TravelManagement.API.Features.Directory.Interfaces;

public interface ICityRepository
{
    Task<IEnumerable<CityDto>> GetAllAsync();

    Task<CityDto?> GetByIdAsync(Guid id);

    Task<CityDto> CreateAsync(CreateCityDto dto);

    Task<IEnumerable<CityDto>> AutocompleteAsync(string term);

    Task<bool> UpdateAsync(Guid id, UpdateCityDto dto);

    Task<bool> DeleteAsync(Guid id);
}