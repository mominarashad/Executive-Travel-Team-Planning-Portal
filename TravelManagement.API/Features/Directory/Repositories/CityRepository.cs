using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Features.Directory.DTOs;
using TravelManagement.API.Features.Directory.Interfaces;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Features.Directory.Repositories;

public class CityRepository : ICityRepository
{
    private readonly ApplicationDbContext _context;

    public CityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CityDto>> AutocompleteAsync(string term)
{
    term = term.Trim().ToLower();

    return await _context.Cities
        .Where(c =>
            c.IsActive &&
            c.Name.ToLower().Contains(term))
        .OrderBy(c => c.Name)
        .Take(10)
        .Select(c => new CityDto
        {
            Id = c.Id,
            Name = c.Name,
            Country = c.Country
        })
        .ToListAsync();
}

    public async Task<IEnumerable<CityDto>> GetAllAsync()
    {
        return await _context.Cities
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new CityDto
            {
                Id = c.Id,
                Name = c.Name,
                Country = c.Country
            })
            .ToListAsync();
    }

    public async Task<CityDto?> GetByIdAsync(Guid id)
    {
        return await _context.Cities
            .Where(c => c.Id == id && c.IsActive)
            .Select(c => new CityDto
            {
                Id = c.Id,
                Name = c.Name,
                Country = c.Country
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CityDto> CreateAsync(CreateCityDto dto)
{
    var exists = await _context.Cities.AnyAsync(c =>
        c.IsActive &&
        c.Name.ToLower() == dto.Name.Trim().ToLower());

    if (exists)
        throw new InvalidOperationException("City already exists.");

    var city = new City
    {
        Id = Guid.NewGuid(),
        Name = dto.Name.Trim(),
        Country = dto.Country.Trim(),
        IsActive = true
    };

    _context.Cities.Add(city);

    await _context.SaveChangesAsync();

    return new CityDto
    {
        Id = city.Id,
        Name = city.Name,
        Country = city.Country
    };
}
    public async Task<bool> UpdateAsync(Guid id, UpdateCityDto dto)
    {
        var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

        if (city == null)
            return false;

        city.Name = dto.Name.Trim();
        city.Country = dto.Country.Trim();

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

        if (city == null)
            return false;

        city.IsActive = false;

        await _context.SaveChangesAsync();

        return true;
    }
}