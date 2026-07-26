using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Features.Hotels.DTOs;
using TravelManagement.API.Features.Hotels.Interfaces;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Features.Hotels.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly ApplicationDbContext _context;

    public HotelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<HotelDto>> GetAllAsync()
    {
        return await _context.Hotels
            .Where(h => h.IsActive)
            .OrderBy(h => h.Name)
            .Select(h => new HotelDto
            {
                Id = h.Id,
                CityId = h.CityId,
                CityName = h.City!.Name,
                Name = h.Name,
                IsCustom = h.IsCustom
            })
            .ToListAsync();
    }

    public async Task<HotelDto?> GetByIdAsync(Guid id)
    {
        return await _context.Hotels
            .Where(h => h.Id == id && h.IsActive)
            .Select(h => new HotelDto
            {
                Id = h.Id,
                CityId = h.CityId,
                CityName = h.City!.Name,
                Name = h.Name,
                IsCustom = h.IsCustom
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<HotelDto>> GetByCityAsync(Guid cityId)
    {
        return await _context.Hotels
            .Where(h => h.IsActive && h.CityId == cityId)
            .OrderBy(h => h.Name)
            .Select(h => new HotelDto
            {
                Id = h.Id,
                CityId = h.CityId,
                CityName = h.City!.Name,
                Name = h.Name,
                IsCustom = h.IsCustom
            })
            .ToListAsync();
    }

    public async Task<HotelDto> CreateAsync(CreateHotelDto dto)
    {
        var cityExists = await _context.Cities
            .AnyAsync(c => c.Id == dto.CityId && c.IsActive);

        if (!cityExists)
            throw new InvalidOperationException("City not found.");

        var duplicate = await _context.Hotels.AnyAsync(h =>
            h.IsActive &&
            h.CityId == dto.CityId &&
            h.Name.ToLower() == dto.Name.Trim().ToLower());

        if (duplicate)
            throw new InvalidOperationException("Hotel already exists for this city.");

        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            CityId = dto.CityId,
            Name = dto.Name.Trim(),
            IsCustom = dto.IsCustom,
            IsActive = true
        };

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        var city = await _context.Cities.FirstAsync(c => c.Id == hotel.CityId);

        return new HotelDto
        {
            Id = hotel.Id,
            CityId = hotel.CityId,
            CityName = city.Name,
            Name = hotel.Name,
            IsCustom = hotel.IsCustom
        };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateHotelDto dto)
    {
        var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.Id == id && h.IsActive);

        if (hotel == null)
            return false;

        hotel.Name = dto.Name.Trim();

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.Id == id && h.IsActive);

        if (hotel == null)
            return false;

        hotel.IsActive = false;

        await _context.SaveChangesAsync();
        return true;
    }
}