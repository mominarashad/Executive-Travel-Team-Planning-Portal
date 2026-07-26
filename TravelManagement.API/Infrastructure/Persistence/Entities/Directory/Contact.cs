using TravelManagement.API.Common;

namespace TravelManagement.API.Infrastructure.Persistence.Entities;

public class Contact : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Organization { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid CityId { get; set; }

    public City City { get; set; } = null!;

    public ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();
}