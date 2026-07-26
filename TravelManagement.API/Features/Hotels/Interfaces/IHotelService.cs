using TravelManagement.API.Features.Hotels.DTOs;

namespace TravelManagement.API.Features.Hotels.Interfaces;

public interface IHotelService
{
    Task<IEnumerable<HotelDto>> GetAllAsync();
    Task<HotelDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<HotelDto>> GetByCityAsync(Guid cityId);
    Task<HotelDto> CreateAsync(CreateHotelDto dto);
    Task<bool> UpdateAsync(Guid id, UpdateHotelDto dto);
    Task<bool> DeleteAsync(Guid id);
}