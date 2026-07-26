using TravelManagement.API.Common;

namespace TravelManagement.API.Infrastructure.Persistence.Entities;

public class TripMember : BaseEntity
{
    public Guid TripId { get; set; }

    public Trip Trip { get; set; } = null!;

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}