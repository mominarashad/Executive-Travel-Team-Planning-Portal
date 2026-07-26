using TravelManagement.API.Common;

namespace TravelManagement.API.Infrastructure.Persistence.Entities;

public class Meeting : BaseEntity
{
    public Guid TripId { get; set; }

    public Trip Trip { get; set; } = null!;

    public Guid ContactId { get; set; }

    public Contact Contact { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public string Priority { get; set; } = "Medium";

    public string Status { get; set; } = "Proposed";

    public TimeOnly? ScheduledTime { get; set; }

    public Guid? ProjectId { get; set; }

    public Project? Project { get; set; }

    public Guid? BusinessEntityId { get; set; }

    public BusinessEntity? BusinessEntity { get; set; }

    public string Agenda { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<MeetingAttendee> MeetingAttendees { get; set; } = new List<MeetingAttendee>();

    public ICollection<MeetingMaterial> Materials { get; set; } = new List<MeetingMaterial>();
}