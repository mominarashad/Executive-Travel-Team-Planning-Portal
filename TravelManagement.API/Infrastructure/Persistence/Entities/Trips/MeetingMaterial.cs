using TravelManagement.API.Common;

namespace TravelManagement.API.Infrastructure.Persistence.Entities;

public class MeetingMaterial : BaseEntity
{
    public Guid MeetingId { get; set; }

    public Meeting Meeting { get; set; } = null!;

    public string Description { get; set; } = string.Empty;

    public Guid? OwnerId { get; set; }

    public User? Owner { get; set; }
}