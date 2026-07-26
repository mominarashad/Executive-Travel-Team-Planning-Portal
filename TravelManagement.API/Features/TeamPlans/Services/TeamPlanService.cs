using TravelManagement.API.Features.TeamPlans.DTOs;
using TravelManagement.API.Features.TeamPlans.Interfaces;

namespace TravelManagement.API.Features.TeamPlans.Services;

public class TeamPlanService : ITeamPlanService
{
    private readonly ITeamPlanRepository _repository;

    public TeamPlanService(ITeamPlanRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<TeamPlanDto>> GetAllAsync()
        => _repository.GetAllAsync();

    public Task<TeamPlanDto?> GetByIdAsync(Guid id)
        => _repository.GetByIdAsync(id);

    public Task<TeamPlanDto> CreateAsync(CreateTeamPlanDto dto)
        => _repository.CreateAsync(dto);

    public Task<IEnumerable<TeamPlanSummaryDto>> GetSummaryAsync(Guid userId)
    => _repository.GetSummaryAsync(userId);

    public Task BulkCreateAsync(BulkCreateTeamPlanDto dto)
        => _repository.BulkCreateAsync(dto);

    public Task<bool> UpdateAsync(Guid id, UpdateTeamPlanDto dto)
        => _repository.UpdateAsync(id, dto);

    public Task<bool> DeleteAsync(Guid id)
        => _repository.DeleteAsync(id);
}