using TravelManagement.API.Common;

namespace TravelManagement.API.Infrastructure.Persistence.Entities;

public class Hotel : BaseEntity
{
    public Guid CityId { get; set; }

    public City? City { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsCustom { get; set; } = true;

    public bool IsActive { get; set; } = true;
}