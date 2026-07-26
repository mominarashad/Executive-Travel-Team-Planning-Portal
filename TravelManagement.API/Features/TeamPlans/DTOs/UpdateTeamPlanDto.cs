using System.ComponentModel.DataAnnotations;

namespace TravelManagement.API.Features.TeamPlans.DTOs;

public class UpdateTeamPlanDto
{
    [Required]
    public Guid UserId { get; set; }

    public Guid? CityId { get; set; }

    [Required]
    public DateOnly FromDate { get; set; }

    [Required]
    public DateOnly ToDate { get; set; }

    [Required]
    public string Type { get; set; } = string.Empty;

    public string ApprovalStatus { get; set; } = "Pending";

    public string Notes { get; set; } = string.Empty;
}