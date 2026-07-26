using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Features.Entities.DTOs;
using TravelManagement.API.Features.Entities.Interfaces;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Features.Entities.Repositories;

public class BusinessEntityRepository : IBusinessEntityRepository
{
    private readonly ApplicationDbContext _context;

    public BusinessEntityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BusinessEntityDto>> GetAllAsync()
    {
        return await _context.BusinessEntities
            .Where(e => e.IsActive)
            .OrderBy(e => e.Name)
            .Select(e => new BusinessEntityDto
            {
                Id = e.Id,
                Name = e.Name,
                IsSystem = e.IsSystem
            })
            .ToListAsync();
    }

    public async Task<BusinessEntityDto?> GetByIdAsync(Guid id)
    {
        return await _context.BusinessEntities
            .Where(e => e.Id == id && e.IsActive)
            .Select(e => new BusinessEntityDto
            {
                Id = e.Id,
                Name = e.Name,
                IsSystem = e.IsSystem
            })
            .FirstOrDefaultAsync();
    }

    public async Task<BusinessEntityDto> CreateAsync(CreateBusinessEntityDto dto)
    {
        var exists = await _context.BusinessEntities.AnyAsync(e =>
            e.IsActive &&
            e.Name.ToLower() == dto.Name.Trim().ToLower());

        if (exists)
            throw new InvalidOperationException("Entity already exists.");

        var entity = new BusinessEntity
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            IsSystem = false,
            IsActive = true
        };

        _context.BusinessEntities.Add(entity);
        await _context.SaveChangesAsync();

        return new BusinessEntityDto
        {
            Id = entity.Id,
            Name = entity.Name,
            IsSystem = entity.IsSystem
        };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateBusinessEntityDto dto)
    {
        var entity = await _context.BusinessEntities.FirstOrDefaultAsync(e => e.Id == id && e.IsActive);

        if (entity == null)
            return false;

        entity.Name = dto.Name.Trim();

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.BusinessEntities.FirstOrDefaultAsync(e => e.Id == id && e.IsActive);

        if (entity == null)
            return false;

        if (entity.IsSystem)
            throw new InvalidOperationException("Cannot delete a system-defined entity.");

        entity.IsActive = false;

        await _context.SaveChangesAsync();
        return true;
    }
}