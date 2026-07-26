using TravelManagement.API.Features.Trips.DTOs;

namespace TravelManagement.API.Features.Trips.Interfaces;

public interface ITripService
{
    Task<IEnumerable<TripDto>> GetAllAsync();
    
    

    Task<TripSearchResultDto> SearchAsync(
    Guid? cityId,
    Guid? projectId,
    Guid? personId,
    string? search);

    Task BulkCreateAsync(BulkCreateTripDto dto);
    Task<TripDto?> GetByIdAsync(Guid id);

    Task<TripDto> CreateAsync(CreateTripDto dto);

    Task<bool> UpdateAsync(Guid id, UpdateTripDto dto);

    Task<bool> DeleteAsync(Guid id);
}