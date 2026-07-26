using TravelManagement.API.Features.TeamPlans.DTOs;

namespace TravelManagement.API.Features.TeamPlans.Interfaces;

public interface ITeamPlanRepository
{
    Task<IEnumerable<TeamPlanDto>> GetAllAsync();

    Task<TeamPlanDto?> GetByIdAsync(Guid id);

    Task<TeamPlanDto> CreateAsync(CreateTeamPlanDto dto);

    Task<bool> UpdateAsync(Guid id, UpdateTeamPlanDto dto);

    Task<bool> DeleteAsync(Guid id);

    Task BulkCreateAsync(BulkCreateTeamPlanDto dto);

    Task<IEnumerable<TeamPlanSummaryDto>> GetSummaryAsync(Guid userId);
}