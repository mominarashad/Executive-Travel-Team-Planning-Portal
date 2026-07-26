using TravelManagement.API.Features.Trips.DTOs;

namespace TravelManagement.API.Features.Trips.Interfaces;

public interface ITripRepository
{
    Task<IEnumerable<TripDto>> GetAllAsync();

    

    Task BulkCreateAsync(BulkCreateTripDto dto);

    Task<TripSearchResultDto> SearchAsync(
    Guid? cityId,
    Guid? projectId,
    Guid? personId,
    string? search);

    Task<TripDto?> GetByIdAsync(Guid id);

    Task<TripDto> CreateAsync(CreateTripDto dto);

    Task<bool> UpdateAsync(Guid id, UpdateTripDto dto);

    Task<bool> DeleteAsync(Guid id);
}