namespace TravelManagement.API.Features.DataManagement.DTOs;

// ---- Per-table export/import shapes (raw fields, no resolved names) ----

public class RoleExportDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

// Included for visibility/backup only — NEVER written back on import.
public class UserExportDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Function { get; set; } = string.Empty;
    public bool IsCeo { get; set; }
    public Guid RoleId { get; set; }
}

public class CityExportDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class ContactExportDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Organization { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public Guid CityId { get; set; }
}

public class ProjectExportDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
}

public class BusinessEntityExportDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
}

public class HotelExportDto
{
    public Guid Id { get; set; }
    public Guid CityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsCustom { get; set; }
    public bool IsActive { get; set; }
}

public class TripExportDto
{
    public Guid Id { get; set; }
    public Guid DestinationCityId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? BusinessEntityId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Hotel { get; set; } = string.Empty;
    public string Transport { get; set; } = string.Empty;
    public string FlightInfo { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class TripMemberExportDto
{
    public Guid Id { get; set; }
    public Guid TripId { get; set; }
    public Guid UserId { get; set; }
}

public class MeetingExportDto
{
    public Guid Id { get; set; }
    public Guid TripId { get; set; }
    public Guid ContactId { get; set; }
    public int DisplayOrder { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public TimeOnly? ScheduledTime { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? BusinessEntityId { get; set; }
    public string Agenda { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class MeetingAttendeeExportDto
{
    public Guid Id { get; set; }
    public Guid MeetingId { get; set; }
    public Guid UserId { get; set; }
}

public class MeetingMaterialExportDto
{
    public Guid Id { get; set; }
    public Guid MeetingId { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? OwnerId { get; set; }
}

public class FlightExportDto
{
    public Guid Id { get; set; }
    public Guid TripId { get; set; }
    public Guid UserId { get; set; }
    public string Airline { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string DepartureAirport { get; set; } = string.Empty;
    public string ArrivalAirport { get; set; } = string.Empty;
    public string Aircraft { get; set; } = string.Empty;
    public string BookingReference { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class TeamPlanEntryExportDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? CityId { get; set; }
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public string Type { get; set; } = string.Empty;
    public string ApprovalStatus { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

// ---- Top-level envelope ----

public class ExportDataDto
{
    public string ExportVersion { get; set; } = "1.0";
    public DateTime ExportedAt { get; set; }

    // Included for visibility/backup only. Ignored on import.
    public List<RoleExportDto> Roles { get; set; } = new();
    public List<UserExportDto> Users { get; set; } = new();

    // Restored on import (full replace).
    public List<CityExportDto> Cities { get; set; } = new();
    public List<ContactExportDto> Contacts { get; set; } = new();
    public List<ProjectExportDto> Projects { get; set; } = new();
    public List<BusinessEntityExportDto> BusinessEntities { get; set; } = new();
    public List<HotelExportDto> Hotels { get; set; } = new();
    public List<TripExportDto> Trips { get; set; } = new();
    public List<TripMemberExportDto> TripMembers { get; set; } = new();
    public List<MeetingExportDto> Meetings { get; set; } = new();
    public List<MeetingAttendeeExportDto> MeetingAttendees { get; set; } = new();
    public List<MeetingMaterialExportDto> MeetingMaterials { get; set; } = new();
    public List<FlightExportDto> Flights { get; set; } = new();
    public List<TeamPlanEntryExportDto> TeamPlanEntries { get; set; } = new();
}