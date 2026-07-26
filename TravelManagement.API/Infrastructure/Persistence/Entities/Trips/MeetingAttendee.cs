using TravelManagement.API.Common;

namespace TravelManagement.API.Infrastructure.Persistence.Entities;

public class MeetingAttendee : BaseEntity
{
    public Guid MeetingId { get; set; }

    public Meeting Meeting { get; set; } = null!;

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}