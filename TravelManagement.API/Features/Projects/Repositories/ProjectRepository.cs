using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Features.Projects.DTOs;
using TravelManagement.API.Features.Projects.Interfaces;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Features.Projects.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllAsync()
    {
        return await _context.Projects
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                IsSystem = p.IsSystem
            })
            .ToListAsync();
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id)
    {
        return await _context.Projects
            .Where(p => p.Id == id && p.IsActive)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                IsSystem = p.IsSystem
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
    {
        var exists = await _context.Projects.AnyAsync(p =>
            p.IsActive &&
            p.Name.ToLower() == dto.Name.Trim().ToLower());

        if (exists)
            throw new InvalidOperationException("Project already exists.");

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            IsSystem = false,
            IsActive = true
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            IsSystem = project.IsSystem
        };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProjectDto dto)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        if (project == null)
            return false;

        project.Name = dto.Name.Trim();

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        if (project == null)
            return false;

        if (project.IsSystem)
            throw new InvalidOperationException("Cannot delete a system-defined project.");

        project.IsActive = false;

        await _context.SaveChangesAsync();
        return true;
    }
}