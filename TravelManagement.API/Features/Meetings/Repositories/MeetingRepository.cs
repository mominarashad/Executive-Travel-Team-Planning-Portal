using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Features.Meetings.DTOs;
using TravelManagement.API.Features.Meetings.Interfaces;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Features.Meetings.Repositories;

public class MeetingRepository : IMeetingRepository
{
    private readonly ApplicationDbContext _context;

    private static readonly string[] ValidPriorities = { "High", "Medium", "Low" };
    private static readonly string[] ValidStatuses =
        { "Proposed", "Requested", "Confirmed", "Tentative", "Declined", "Completed" };

    public MeetingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // ---------- shared validation ----------

    private static string NormalizeChoice(string value, string[] validValues, string fieldName)
    {
        var match = validValues.FirstOrDefault(v =>
            v.Equals(value?.Trim(), StringComparison.OrdinalIgnoreCase));

        if (match == null)
            throw new InvalidOperationException(
                $"Invalid {fieldName} '{value}'. Must be one of: {string.Join(", ", validValues)}.");

        return match;
    }

    private async Task ValidateReferencesAsync(
        Guid tripId,
        Guid contactId,
        Guid? projectId,
        Guid? businessEntityId,
        List<Guid> attendeeIds,
        List<CreateMeetingMaterialDto> materials)
    {
        var tripExists = await _context.Trips.AnyAsync(t => t.Id == tripId && t.IsActive);
        if (!tripExists)
            throw new InvalidOperationException("Trip not found.");

        var contactExists = await _context.Contacts.AnyAsync(c => c.Id == contactId);
        if (!contactExists)
            throw new InvalidOperationException("Contact not found.");

        if (projectId.HasValue)
        {
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId.Value && p.IsActive);
            if (!projectExists)
                throw new InvalidOperationException("Project not found.");
        }

        if (businessEntityId.HasValue)
        {
            var entityExists = await _context.BusinessEntities.AnyAsync(e => e.Id == businessEntityId.Value && e.IsActive);
            if (!entityExists)
                throw new InvalidOperationException("Business entity not found.");
        }

        if (attendeeIds != null && attendeeIds.Count > 0)
        {
            var distinctIds = attendeeIds.Distinct().ToList();
            var foundCount = await _context.Users.CountAsync(u => distinctIds.Contains(u.Id));
            if (foundCount != distinctIds.Count)
                throw new InvalidOperationException("One or more attendees were not found.");
        }

        if (materials != null)
        {
            var ownerIds = materials
                .Where(m => m.OwnerId.HasValue)
                .Select(m => m.OwnerId!.Value)
                .Distinct()
                .ToList();

            if (ownerIds.Count > 0)
            {
                var foundCount = await _context.Users.CountAsync(u => ownerIds.Contains(u.Id));
                if (foundCount != ownerIds.Count)
                    throw new InvalidOperationException("One or more material owners were not found.");
            }
        }
    }

    private async Task EnsureNoDisplayOrderCollisionAsync(Guid tripId, int displayOrder, Guid? excludeMeetingId)
    {
        var collision = await _context.Meetings.AnyAsync(m =>
            m.IsActive
            && m.TripId == tripId
            && m.DisplayOrder == displayOrder
            && (excludeMeetingId == null || m.Id != excludeMeetingId));

        if (collision)
            throw new InvalidOperationException(
                $"Display order {displayOrder} is already used by another meeting on this trip.");
    }

    // ---------- reads (unchanged) ----------

    public async Task<IEnumerable<MeetingDto>> GetAllAsync()
    {
        return await _context.Meetings
            .Include(m => m.Contact)
            .Include(m => m.MeetingAttendees)
            .Include(m => m.Materials).ThenInclude(x => x.Owner)
            .Where(m => m.IsActive)
            .OrderBy(m => m.DisplayOrder)
            .Select(m => new MeetingDto
            {
                Id = m.Id,
                TripId = m.TripId,
                ContactId = m.ContactId,
                ContactName = m.Contact.Name,
                DisplayOrder = m.DisplayOrder,
                Priority = m.Priority,
                Status = m.Status,
                ScheduledTime = m.ScheduledTime,
                ProjectId = m.ProjectId,
                ProjectName = m.Project != null ? m.Project.Name : string.Empty,
                BusinessEntityId = m.BusinessEntityId,
                BusinessEntityName = m.BusinessEntity != null ? m.BusinessEntity.Name : string.Empty,
                Agenda = m.Agenda,
                AttendeeIds = m.MeetingAttendees.Select(a => a.UserId).ToList(),
                Materials = m.Materials.Select(x => new MeetingMaterialDto
                {
                    Id = x.Id,
                    Description = x.Description,
                    OwnerId = x.OwnerId,
                    OwnerName = x.Owner != null ? x.Owner.Name : null
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<MeetingDto?> GetByIdAsync(Guid id)
    {
        return await _context.Meetings
            .Include(m => m.Contact)
            .Include(m => m.MeetingAttendees)
            .Include(m => m.Materials).ThenInclude(x => x.Owner)
            .Where(m => m.Id == id && m.IsActive)
            .Select(m => new MeetingDto
            {
                Id = m.Id,
                TripId = m.TripId,
                ContactId = m.ContactId,
                ContactName = m.Contact.Name,
                DisplayOrder = m.DisplayOrder,
                Priority = m.Priority,
                Status = m.Status,
                ScheduledTime = m.ScheduledTime,
                ProjectId = m.ProjectId,
                ProjectName = m.Project != null ? m.Project.Name : string.Empty,
                BusinessEntityId = m.BusinessEntityId,
                BusinessEntityName = m.BusinessEntity != null ? m.BusinessEntity.Name : string.Empty,
                Agenda = m.Agenda,
                AttendeeIds = m.MeetingAttendees.Select(a => a.UserId).ToList(),
                Materials = m.Materials.Select(x => new MeetingMaterialDto
                {
                    Id = x.Id,
                    Description = x.Description,
                    OwnerId = x.OwnerId,
                    OwnerName = x.Owner != null ? x.Owner.Name : null
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    // ---------- writes (validated) ----------

    public async Task<MeetingDto> CreateAsync(CreateMeetingDto dto)
    {
        var priority = NormalizeChoice(dto.Priority, ValidPriorities, "priority");
        var status = NormalizeChoice(dto.Status, ValidStatuses, "status");

        await ValidateReferencesAsync(
            dto.TripId, dto.ContactId, dto.ProjectId, dto.BusinessEntityId,
            dto.AttendeeIds, dto.Materials);

        await EnsureNoDisplayOrderCollisionAsync(dto.TripId, dto.DisplayOrder, excludeMeetingId: null);

        var meeting = new Meeting
        {
            Id = Guid.NewGuid(),
            TripId = dto.TripId,
            ContactId = dto.ContactId,
            DisplayOrder = dto.DisplayOrder,
            Priority = priority,
            Status = status,
            ScheduledTime = dto.ScheduledTime,
            ProjectId = dto.ProjectId,
            BusinessEntityId = dto.BusinessEntityId,
            Agenda = dto.Agenda,
            IsActive = true
        };

        _context.Meetings.Add(meeting);
        await _context.SaveChangesAsync();

        foreach (var attendeeId in dto.AttendeeIds.Distinct())
        {
            _context.MeetingAttendees.Add(new MeetingAttendee
            {
                Id = Guid.NewGuid(),
                MeetingId = meeting.Id,
                UserId = attendeeId
            });
        }

        foreach (var material in dto.Materials)
        {
            _context.MeetingMaterials.Add(new MeetingMaterial
            {
                Id = Guid.NewGuid(),
                MeetingId = meeting.Id,
                Description = material.Description,
                OwnerId = material.OwnerId
            });
        }

        await _context.SaveChangesAsync();

        return await GetByIdAsync(meeting.Id)
            ?? throw new Exception("Meeting could not be loaded.");
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateMeetingDto dto)
    {
        var meeting = await _context.Meetings.FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

        if (meeting == null)
            return false;

        var priority = NormalizeChoice(dto.Priority, ValidPriorities, "priority");
        var status = NormalizeChoice(dto.Status, ValidStatuses, "status");

        await ValidateReferencesAsync(
            dto.TripId, dto.ContactId, dto.ProjectId, dto.BusinessEntityId,
            dto.AttendeeIds, dto.Materials);

        await EnsureNoDisplayOrderCollisionAsync(dto.TripId, dto.DisplayOrder, excludeMeetingId: meeting.Id);

        meeting.TripId = dto.TripId;
        meeting.ContactId = dto.ContactId;
        meeting.DisplayOrder = dto.DisplayOrder;
        meeting.Priority = priority;
        meeting.Status = status;
        meeting.ScheduledTime = dto.ScheduledTime;
        meeting.ProjectId = dto.ProjectId;
        meeting.BusinessEntityId = dto.BusinessEntityId;
        meeting.Agenda = dto.Agenda;

        var attendees = await _context.MeetingAttendees.Where(x => x.MeetingId == meeting.Id).ToListAsync();
        _context.MeetingAttendees.RemoveRange(attendees);

        foreach (var attendeeId in dto.AttendeeIds.Distinct())
        {
            _context.MeetingAttendees.Add(new MeetingAttendee
            {
                Id = Guid.NewGuid(),
                MeetingId = meeting.Id,
                UserId = attendeeId
            });
        }

        var materials = await _context.MeetingMaterials.Where(x => x.MeetingId == meeting.Id).ToListAsync();
        _context.MeetingMaterials.RemoveRange(materials);

        foreach (var material in dto.Materials)
        {
            _context.MeetingMaterials.Add(new MeetingMaterial
            {
                Id = Guid.NewGuid(),
                MeetingId = meeting.Id,
                Description = material.Description,
                OwnerId = material.OwnerId
            });
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var meeting = await _context.Meetings.FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

        if (meeting == null)
            return false;

        meeting.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }
}