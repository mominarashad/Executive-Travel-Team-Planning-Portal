using TravelManagement.API.Features.Directory.DTOs;
using TravelManagement.API.Features.Directory.Interfaces;

namespace TravelManagement.API.Features.Directory.Services;

public class ContactService : IContactService
{
    private readonly IContactRepository _repository;

    public ContactService(IContactRepository repository)
    {
        _repository = repository;
    }
    public async Task<IEnumerable<ContactDto>> GetByCityAsync(Guid cityId)
{
    return await _repository.GetByCityAsync(cityId);

    
}
    public async Task<IEnumerable<ContactDto>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<ContactDto?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<ContactDto> CreateAsync(CreateContactDto dto)
    {
        return await _repository.CreateAsync(dto);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateContactDto dto)
    {
        return await _repository.UpdateAsync(id, dto);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }
}