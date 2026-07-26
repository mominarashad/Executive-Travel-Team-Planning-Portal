namespace TravelManagement.API.Features.TeamPlans.DTOs;

public class TeamPlanDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public Guid? CityId { get; set; }

    public string? CityName { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public string Type { get; set; } = string.Empty;

    public string ApprovalStatus { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;
}