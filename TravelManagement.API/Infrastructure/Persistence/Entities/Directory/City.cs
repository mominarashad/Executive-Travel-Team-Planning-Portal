using TravelManagement.API.Common;

namespace TravelManagement.API.Infrastructure.Persistence.Entities;

public class City : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public string Country { get; set; } = string.Empty;

    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
}