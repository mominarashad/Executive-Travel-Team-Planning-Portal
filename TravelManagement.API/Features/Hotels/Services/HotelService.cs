using TravelManagement.API.Features.Hotels.DTOs;
using TravelManagement.API.Features.Hotels.Interfaces;

namespace TravelManagement.API.Features.Hotels.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _repository;

    public HotelService(IHotelRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<HotelDto>> GetAllAsync() =>
        await _repository.GetAllAsync();

    public async Task<HotelDto?> GetByIdAsync(Guid id) =>
        await _repository.GetByIdAsync(id);

    public async Task<IEnumerable<HotelDto>> GetByCityAsync(Guid cityId) =>
        await _repository.GetByCityAsync(cityId);

    public async Task<HotelDto> CreateAsync(CreateHotelDto dto) =>
        await _repository.CreateAsync(dto);

    public async Task<bool> UpdateAsync(Guid id, UpdateHotelDto dto) =>
        await _repository.UpdateAsync(id, dto);

    public async Task<bool> DeleteAsync(Guid id) =>
        await _repository.DeleteAsync(id);
}