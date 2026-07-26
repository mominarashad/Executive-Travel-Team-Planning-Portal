using TravelManagement.API.Features.Flights.DTOs;
using TravelManagement.API.Features.Flights.Interfaces;

namespace TravelManagement.API.Features.Flights.Services;

public class FlightService : IFlightService
{
    private readonly IFlightRepository _repository;

    public FlightService(IFlightRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<FlightDto>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task<FlightDto?> GetByIdAsync(Guid id)
    {
        return _repository.GetByIdAsync(id);
    }

    public Task<FlightDto> CreateAsync(CreateFlightDto dto)
    {
        return _repository.CreateAsync(dto);
    }

    public Task<bool> UpdateAsync(Guid id, UpdateFlightDto dto)
    {
        return _repository.UpdateAsync(id, dto);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return _repository.DeleteAsync(id);
    }
}