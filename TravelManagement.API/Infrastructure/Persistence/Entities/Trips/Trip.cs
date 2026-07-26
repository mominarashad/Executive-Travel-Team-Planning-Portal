using TravelManagement.API.Common;

namespace TravelManagement.API.Infrastructure.Persistence.Entities;
using TravelManagement.API.Infrastructure.Persistence.Entities.Flights;
public class Trip : BaseEntity
{
    public Guid DestinationCityId { get; set; }

    public City DestinationCity { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public Guid? ProjectId { get; set; }

    public Project? Project { get; set; }

    public Guid? BusinessEntityId { get; set; }

    public BusinessEntity? BusinessEntity { get; set; }

    public string Status { get; set; } = "Planned";

    

    public string Hotel { get; set; } = string.Empty;

    public string Transport { get; set; } = string.Empty;

    public string FlightInfo { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();

    public ICollection<TripMember> TripMembers { get; set; } = new List<TripMember>();

    public ICollection<Flight> Flights { get; set; } = new List<Flight>();
}