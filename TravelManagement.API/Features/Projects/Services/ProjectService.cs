using TravelManagement.API.Features.Projects.DTOs;
using TravelManagement.API.Features.Projects.Interfaces;

namespace TravelManagement.API.Features.Projects.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repository;

    public ProjectService(IProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllAsync() => await _repository.GetAllAsync();
    public async Task<ProjectDto?> GetByIdAsync(Guid id) => await _repository.GetByIdAsync(id);
    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto) => await _repository.CreateAsync(dto);
    public async Task<bool> UpdateAsync(Guid id, UpdateProjectDto dto) => await _repository.UpdateAsync(id, dto);
    public async Task<bool> DeleteAsync(Guid id) => await _repository.DeleteAsync(id);
}