using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Features.Directory.DTOs;
using TravelManagement.API.Features.Directory.Interfaces;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Features.Directory.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly ApplicationDbContext _context;

    public ContactRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ContactDto>> GetAllAsync()
    {
        return await _context.Contacts
            .Include(c => c.City)
            .Where(c => c.IsActive)
            .OrderBy(c => c.City.Name)
            .ThenBy(c => c.SortOrder)
            .Select(c => new ContactDto
            {
                Id = c.Id,
                Name = c.Name,
                Organization = c.Organization,
                Role = c.Role,
                Email = c.Email,
                Phone = c.Phone,
                SortOrder = c.SortOrder,
                CityId = c.CityId,
                CityName = c.City.Name
            })
            .ToListAsync();
    }

    public async Task<ContactDto?> GetByIdAsync(Guid id)
    {
        return await _context.Contacts
            .Include(c => c.City)
            .Where(c => c.Id == id && c.IsActive)
            .Select(c => new ContactDto
            {
                Id = c.Id,
                Name = c.Name,
                Organization = c.Organization,
                Role = c.Role,
                Email = c.Email,
                Phone = c.Phone,
                SortOrder = c.SortOrder,
                CityId = c.CityId,
                CityName = c.City.Name
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ContactDto> CreateAsync(CreateContactDto dto)
    {
        var contact = new Contact
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Organization = dto.Organization,
            Role = dto.Role,
            Email = dto.Email,
            Phone = dto.Phone,
            SortOrder = dto.SortOrder,
            CityId = dto.CityId,
            IsActive = true
        };

        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();

        var city = await _context.Cities.FindAsync(dto.CityId);

        return new ContactDto
        {
            Id = contact.Id,
            Name = contact.Name,
            Organization = contact.Organization,
            Role = contact.Role,
            Email = contact.Email,
            Phone = contact.Phone,
            SortOrder = contact.SortOrder,
            CityId = contact.CityId,
            CityName = city?.Name ?? string.Empty
        };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateContactDto dto)
    {
        var contact = await _context.Contacts
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

        if (contact == null)
            return false;

        contact.Name = dto.Name;
        contact.Organization = dto.Organization;
        contact.Role = dto.Role;
        contact.Email = dto.Email;
        contact.Phone = dto.Phone;
        contact.SortOrder = dto.SortOrder;
        contact.CityId = dto.CityId;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<ContactDto>> GetByCityAsync(Guid cityId)
{
    return await _context.Contacts
        .Include(c => c.City)
        .Where(c => c.IsActive && c.CityId == cityId)
        .OrderBy(c => c.SortOrder)
        .Select(c => new ContactDto
        {
            Id = c.Id,
            Name = c.Name,
            Organization = c.Organization,
            Role = c.Role,
            Email = c.Email,
            Phone = c.Phone,
            SortOrder = c.SortOrder,
            CityId = c.CityId,
            CityName = c.City.Name
        })
        .ToListAsync();
}

    public async Task<bool> DeleteAsync(Guid id)
    {
        var contact = await _context.Contacts
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

        if (contact == null)
            return false;

        contact.IsActive = false;

        await _context.SaveChangesAsync();

        return true;
    }
}