using TravelManagement.API.Features.TeamPlans.DTOs;

namespace TravelManagement.API.Features.TeamPlans.Interfaces;

public interface ITeamPlanService
{
    Task<IEnumerable<TeamPlanDto>> GetAllAsync();

    Task<TeamPlanDto?> GetByIdAsync(Guid id);

    Task<TeamPlanDto> CreateAsync(CreateTeamPlanDto dto);

    Task BulkCreateAsync(BulkCreateTeamPlanDto dto);

    Task<bool> UpdateAsync(Guid id, UpdateTeamPlanDto dto);

    Task<IEnumerable<TeamPlanSummaryDto>> GetSummaryAsync(Guid userId);

    Task<bool> DeleteAsync(Guid id);
}