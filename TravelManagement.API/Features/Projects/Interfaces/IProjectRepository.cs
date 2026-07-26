using TravelManagement.API.Features.Projects.DTOs;

namespace TravelManagement.API.Features.Projects.Interfaces;

public interface IProjectRepository
{
    Task<IEnumerable<ProjectDto>> GetAllAsync();
    Task<ProjectDto?> GetByIdAsync(Guid id);
    Task<ProjectDto> CreateAsync(CreateProjectDto dto);
    Task<bool> UpdateAsync(Guid id, UpdateProjectDto dto);
    Task<bool> DeleteAsync(Guid id);
}