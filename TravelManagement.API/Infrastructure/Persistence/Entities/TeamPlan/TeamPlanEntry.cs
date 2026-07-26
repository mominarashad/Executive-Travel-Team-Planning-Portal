using TravelManagement.API.Common;

namespace TravelManagement.API.Infrastructure.Persistence.Entities;

public class TeamPlanEntry : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid? CityId { get; set; }
    public City? City { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public string Type { get; set; } = string.Empty;
    // Trip / Vacation / Remote / Option

    public string ApprovalStatus { get; set; } = "Pending";
    // Pending / Approved / Rejected

    public string Notes { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}