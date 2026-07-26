using TravelManagement.API.Features.Entities.DTOs;
using TravelManagement.API.Features.Entities.Interfaces;

namespace TravelManagement.API.Features.Entities.Services;

public class BusinessEntityService : IBusinessEntityService
{
    private readonly IBusinessEntityRepository _repository;

    public BusinessEntityService(IBusinessEntityRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<BusinessEntityDto>> GetAllAsync() => await _repository.GetAllAsync();
    public async Task<BusinessEntityDto?> GetByIdAsync(Guid id) => await _repository.GetByIdAsync(id);
    public async Task<BusinessEntityDto> CreateAsync(CreateBusinessEntityDto dto) => await _repository.CreateAsync(dto);
    public async Task<bool> UpdateAsync(Guid id, UpdateBusinessEntityDto dto) => await _repository.UpdateAsync(id, dto);
    public async Task<bool> DeleteAsync(Guid id) => await _repository.DeleteAsync(id);
}