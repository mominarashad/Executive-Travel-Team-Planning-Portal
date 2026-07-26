using System.ComponentModel.DataAnnotations;

namespace TravelManagement.API.Features.TeamPlans.DTOs;

public class BulkCreateTeamPlanDto
{
    [Required]
    public List<Guid> UserIds { get; set; } = new();

    public Guid? CityId { get; set; }

    public DateOnly? FromDate { get; set; }

    public DateOnly? ToDate { get; set; }

    [Required]
    public string Type { get; set; } = string.Empty;

    public string? ApprovalStatus { get; set; }

    public string? Notes { get; set; }
}