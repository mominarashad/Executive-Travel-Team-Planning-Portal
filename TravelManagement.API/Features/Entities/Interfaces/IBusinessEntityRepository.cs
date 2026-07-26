using TravelManagement.API.Features.Entities.DTOs;

namespace TravelManagement.API.Features.Entities.Interfaces;

public interface IBusinessEntityRepository
{
    Task<IEnumerable<BusinessEntityDto>> GetAllAsync();
    Task<BusinessEntityDto?> GetByIdAsync(Guid id);
    Task<BusinessEntityDto> CreateAsync(CreateBusinessEntityDto dto);
    Task<bool> UpdateAsync(Guid id, UpdateBusinessEntityDto dto);
    Task<bool> DeleteAsync(Guid id);
}