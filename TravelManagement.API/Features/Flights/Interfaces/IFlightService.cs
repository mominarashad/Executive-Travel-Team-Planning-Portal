using TravelManagement.API.Features.Flights.DTOs;

namespace TravelManagement.API.Features.Flights.Interfaces;

public interface IFlightService
{
    Task<IEnumerable<FlightDto>> GetAllAsync();

    Task<FlightDto?> GetByIdAsync(Guid id);

    Task<FlightDto> CreateAsync(CreateFlightDto dto);

    Task<bool> UpdateAsync(Guid id, UpdateFlightDto dto);

    Task<bool> DeleteAsync(Guid id);
}