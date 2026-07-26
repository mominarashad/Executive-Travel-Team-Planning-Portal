using TravelManagement.API.Common;

namespace TravelManagement.API.Infrastructure.Persistence.Entities;

public class BusinessEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public bool IsSystem { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Trip> Trips { get; set; } = new List<Trip>();

    public ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();
}