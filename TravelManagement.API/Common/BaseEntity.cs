namespace TravelManagement.API.Common;

// the reason we created seprate base entity is bcz of the fact we might need to create deleted_at and imagine repeating these four properties in 15 different entity classes.
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid(); //id har table mai use hni har jagah use hni thats why we created 1 and will be used by all entities

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}