using TravelManagement.API.Features.Directory.DTOs;

namespace TravelManagement.API.Features.Directory.Interfaces;

public interface IContactService
{
    Task<IEnumerable<ContactDto>> GetAllAsync();

    Task<ContactDto?> GetByIdAsync(Guid id);

    Task<ContactDto> CreateAsync(CreateContactDto dto);

    Task<IEnumerable<ContactDto>> GetByCityAsync(Guid cityId);

    Task<bool> UpdateAsync(Guid id, UpdateContactDto dto);

    

    Task<bool> DeleteAsync(Guid id);
}