using TravelManagement.API.Common;
using TravelManagement.API.Infrastructure.Persistence.Entities.Flights;
namespace TravelManagement.API.Infrastructure.Persistence.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Function { get; set; } = string.Empty;

    public bool IsCeo { get; set; }

    public Guid RoleId { get; set; }

    public Role Role { get; set; } = null!;

    
    // Trips this user accompanies
    public ICollection<TripMember> TripMembers { get; set; } = new List<TripMember>();

    // Meetings this user attends
    public ICollection<MeetingAttendee> MeetingAttendees { get; set; } = new List<MeetingAttendee>();

    // Materials owned by this user
    public ICollection<MeetingMaterial> OwnedMaterials { get; set; } = new List<MeetingMaterial>();

    public ICollection<TeamPlanEntry> TeamPlanEntries { get; set; } = new List<TeamPlanEntry>();

    public ICollection<Flight> Flights { get; set; } = new List<Flight>();
}