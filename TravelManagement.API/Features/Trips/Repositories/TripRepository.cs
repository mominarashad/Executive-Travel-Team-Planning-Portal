using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Features.Trips.DTOs;
using TravelManagement.API.Features.Trips.Interfaces;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;

namespace TravelManagement.API.Features.Trips.Repositories;

public class TripRepository : ITripRepository
{
    private readonly ApplicationDbContext _context;

    public TripRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // ---------- NEW: shared validation helpers ----------

    private async Task ValidateTripReferencesAsync(
        Guid destinationCityId,
        Guid? projectId,
        Guid? businessEntityId,
        List<Guid> teamMemberIds)
    {
        var cityExists = await _context.Cities
            .AnyAsync(c => c.Id == destinationCityId && c.IsActive);

        if (!cityExists)
            throw new InvalidOperationException("Destination city not found.");

        if (projectId.HasValue)
        {
            var projectExists = await _context.Projects
                .AnyAsync(p => p.Id == projectId.Value && p.IsActive);

            if (!projectExists)
                throw new InvalidOperationException("Project not found.");
        }

        if (businessEntityId.HasValue)
        {
            var entityExists = await _context.BusinessEntities
                .AnyAsync(e => e.Id == businessEntityId.Value && e.IsActive);

            if (!entityExists)
                throw new InvalidOperationException("Business entity not found.");
        }

        if (teamMemberIds != null && teamMemberIds.Count > 0)
        {
            var distinctIds = teamMemberIds.Distinct().ToList();
            var foundCount = await _context.Users
                .CountAsync(u => distinctIds.Contains(u.Id));

            if (foundCount != distinctIds.Count)
                throw new InvalidOperationException("One or more team members were not found.");
        }
    }

    private async Task EnsureNoDoubleBookingAsync(
        Guid userId,
        DateOnly start,
        DateOnly end,
        Guid? excludeTripId)
    {
        var confirmedTripConflict = await _context.Trips
            .Where(t => t.IsActive
                && t.Status == "Confirmed"
                && (excludeTripId == null || t.Id != excludeTripId)
                && t.StartDate <= end && start <= t.EndDate
                && (t.TripMembers.Any(tm => tm.UserId == userId)
                    || t.Meetings.Any(m => m.MeetingAttendees.Any(a => a.UserId == userId))))
            .Select(t => new { t.DestinationCity.Name, t.StartDate, t.EndDate })
            .FirstOrDefaultAsync();

        if (confirmedTripConflict != null)
            throw new InvalidOperationException(
                $"Team member is already confirmed on a trip to {confirmedTripConflict.Name} " +
                $"({confirmedTripConflict.StartDate:yyyy-MM-dd} to {confirmedTripConflict.EndDate:yyyy-MM-dd}) " +
                "that overlaps these dates.");

        var planConflict = await _context.TeamPlanEntries
            .Where(e => e.IsActive
                && e.UserId == userId
                && e.FromDate <= end && start <= e.ToDate
                && (e.Type == "Trip"
                    || e.Type == "Remote"
                    || (e.Type == "Vacation" && e.ApprovalStatus == "Approved")))
            .FirstOrDefaultAsync();

        if (planConflict != null)
            throw new InvalidOperationException(
                $"Team member already has a {planConflict.Type.ToLower()} entry " +
                $"({planConflict.FromDate:yyyy-MM-dd} to {planConflict.ToDate:yyyy-MM-dd}) " +
                "that overlaps these dates.");
    }

    private async Task ValidateAndCheckAsync(
        Guid destinationCityId,
        DateOnly start,
        DateOnly end,
        Guid? projectId,
        Guid? businessEntityId,
        List<Guid> teamMemberIds,
        Guid? excludeTripId)
    {
        if (end < start)
            throw new InvalidOperationException("End date cannot be earlier than start date.");

        await ValidateTripReferencesAsync(destinationCityId, projectId, businessEntityId, teamMemberIds);

        foreach (var userId in teamMemberIds.Distinct())
        {
            await EnsureNoDoubleBookingAsync(userId, start, end, excludeTripId);
        }
    }

    // ---------- existing methods (GetAllAsync, SearchAsync unchanged) ----------

    public async Task<IEnumerable<TripDto>> GetAllAsync()
    {
        return await _context.Trips
            .Where(t => t.IsActive)
            .OrderBy(t => t.StartDate)
            .Select(t => new TripDto
            {
                Id = t.Id,
                DestinationCityId = t.DestinationCityId,
                DestinationCity = t.DestinationCity.Name,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                ProjectId = t.ProjectId,
                ProjectName = t.Project != null ? t.Project.Name : string.Empty,
                BusinessEntityId = t.BusinessEntityId,
                BusinessEntityName = t.BusinessEntity != null ? t.BusinessEntity.Name : string.Empty,
                Status = t.Status,
                Hotel = t.Hotel,
                Transport = t.Transport,
                Notes = t.Notes,
                TeamMemberIds = t.TripMembers.Select(tm => tm.UserId).ToList()
            })
            .ToListAsync();
    }

    public async Task<TripSearchResultDto> SearchAsync(
        Guid? cityId, Guid? projectId, Guid? personId, string? search)
    {
        var query = _context.Trips.Where(t => t.IsActive).AsQueryable();

        if (cityId.HasValue)
            query = query.Where(t => t.DestinationCityId == cityId.Value);

        if (projectId.HasValue)
            query = query.Where(t => t.ProjectId == projectId.Value);

        if (personId.HasValue)
            query = query.Where(t => t.TripMembers.Any(tm => tm.UserId == personId.Value));

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(t =>
                t.DestinationCity.Name.ToLower().Contains(search)
                || t.Status.ToLower().Contains(search)
                || t.Hotel.ToLower().Contains(search)
                || t.Transport.ToLower().Contains(search)
                || t.Notes.ToLower().Contains(search)
                || (t.Project != null && t.Project.Name.ToLower().Contains(search))
                || (t.BusinessEntity != null && t.BusinessEntity.Name.ToLower().Contains(search)));
        }

        var trips = await query
            .OrderBy(t => t.StartDate)
            .Select(t => new TripDto
            {
                Id = t.Id,
                DestinationCityId = t.DestinationCityId,
                DestinationCity = t.DestinationCity.Name,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                ProjectId = t.ProjectId,
                ProjectName = t.Project != null ? t.Project.Name : string.Empty,
                BusinessEntityId = t.BusinessEntityId,
                BusinessEntityName = t.BusinessEntity != null ? t.BusinessEntity.Name : string.Empty,
                Status = t.Status,
                Hotel = t.Hotel,
                Transport = t.Transport,
                FlightInfo = t.FlightInfo,
                Notes = t.Notes,
                TeamMemberIds = t.TripMembers.Select(tm => tm.UserId).ToList()
            })
            .ToListAsync();

        var today = DateOnly.FromDateTime(DateTime.Today);

        return new TripSearchResultDto
        {
            Upcoming = trips.Where(t => t.StartDate >= today).ToList(),
            Past = trips.Where(t => t.StartDate < today).ToList()
        };
    }

    // ---------- BulkCreateAsync: now validated, saved sequentially ----------

    public async Task BulkCreateAsync(BulkCreateTripDto dto)
    {
        foreach (var tripDto in dto.Trips)
        {
            await ValidateAndCheckAsync(
                tripDto.DestinationCityId,
                tripDto.StartDate,
                tripDto.EndDate,
                tripDto.ProjectId,
                tripDto.BusinessEntityId,
                tripDto.TeamMemberIds,
                excludeTripId: null);

            var trip = new Trip
            {
                Id = Guid.NewGuid(),
                DestinationCityId = tripDto.DestinationCityId,
                StartDate = tripDto.StartDate,
                EndDate = tripDto.EndDate,
                ProjectId = tripDto.ProjectId,
                BusinessEntityId = tripDto.BusinessEntityId,
                Status = tripDto.Status,
                Hotel = tripDto.Hotel,
                Transport = tripDto.Transport,
                FlightInfo = tripDto.FlightInfo,
                Notes = tripDto.Notes,
                IsActive = true
            };

            _context.Trips.Add(trip);

            foreach (var userId in tripDto.TeamMemberIds.Distinct())
            {
                _context.TripMembers.Add(new TripMember
                {
                    Id = Guid.NewGuid(),
                    TripId = trip.Id,
                    UserId = userId
                });
            }

            // Saved per-trip (not batched at the end) so each subsequent trip's
            // validation check sees prior trips in this same bulk request.
            await _context.SaveChangesAsync();
        }
    }

    public async Task<TripDto?> GetByIdAsync(Guid id)
    {
        return await _context.Trips
            .Where(t => t.Id == id && t.IsActive)
            .Select(t => new TripDto
            {
                Id = t.Id,
                DestinationCityId = t.DestinationCityId,
                DestinationCity = t.DestinationCity.Name,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                ProjectId = t.ProjectId,
                ProjectName = t.Project != null ? t.Project.Name : string.Empty,
                BusinessEntityId = t.BusinessEntityId,
                BusinessEntityName = t.BusinessEntity != null ? t.BusinessEntity.Name : string.Empty,
                Status = t.Status,
                Hotel = t.Hotel,
                Transport = t.Transport,
                Notes = t.Notes,
                TeamMemberIds = t.TripMembers.Select(tm => tm.UserId).ToList(),
                TeamMembers = t.TripMembers
                    .Select(tm => new TripTeamMemberDto
                    {
                        Id = tm.User.Id,
                        Name = tm.User.Name,
                        Title = tm.User.Title
                    })
                    .ToList(),
                Meetings = t.Meetings
                    .OrderBy(m => m.DisplayOrder)
                    .Select(m => new TripMeetingDto
                    {
                        Id = m.Id,
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
                        Attendees = m.MeetingAttendees
                            .Select(a => new TripMeetingAttendeeDto { Id = a.User.Id, Name = a.User.Name })
                            .ToList(),
                        Materials = m.Materials
                            .Select(mat => new TripMeetingMaterialDto
                            {
                                Id = mat.Id,
                                Description = mat.Description,
                                OwnerName = mat.Owner != null ? mat.Owner.Name : null
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<TripDto> CreateAsync(CreateTripDto dto)
    {
        await ValidateAndCheckAsync(
            dto.DestinationCityId,
            dto.StartDate,
            dto.EndDate,
            dto.ProjectId,
            dto.BusinessEntityId,
            dto.TeamMemberIds,
            excludeTripId: null);

        var trip = new Trip
        {
            Id = Guid.NewGuid(),
            DestinationCityId = dto.DestinationCityId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            ProjectId = dto.ProjectId,
            BusinessEntityId = dto.BusinessEntityId,
            Status = dto.Status,
            Hotel = dto.Hotel,
            Transport = dto.Transport,
            Notes = dto.Notes,
            IsActive = true
        };

        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        foreach (var userId in dto.TeamMemberIds.Distinct())
        {
            _context.TripMembers.Add(new TripMember
            {
                Id = Guid.NewGuid(),
                TripId = trip.Id,
                UserId = userId
            });
        }

        await _context.SaveChangesAsync();

        return new TripDto
        {
            Id = trip.Id,
            DestinationCityId = trip.DestinationCityId,
            DestinationCity = (await _context.Cities.FindAsync(trip.DestinationCityId))?.Name ?? string.Empty,
            StartDate = trip.StartDate,
            EndDate = trip.EndDate,
            ProjectId = trip.ProjectId,
            ProjectName = trip.ProjectId.HasValue
                ? (await _context.Projects.FindAsync(trip.ProjectId))?.Name ?? string.Empty
                : string.Empty,
            BusinessEntityId = trip.BusinessEntityId,
            BusinessEntityName = trip.BusinessEntityId.HasValue
                ? (await _context.BusinessEntities.FindAsync(trip.BusinessEntityId))?.Name ?? string.Empty
                : string.Empty,
            Status = trip.Status,
            Hotel = trip.Hotel,
            Transport = trip.Transport,
            Notes = trip.Notes,
            TeamMemberIds = dto.TeamMemberIds
        };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateTripDto dto)
    {
        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.Id == id && t.IsActive);

        if (trip == null)
            return false;

        await ValidateAndCheckAsync(
            dto.DestinationCityId,
            dto.StartDate,
            dto.EndDate,
            dto.ProjectId,
            dto.BusinessEntityId,
            dto.TeamMemberIds,
            excludeTripId: trip.Id);

        trip.DestinationCityId = dto.DestinationCityId;
        trip.StartDate = dto.StartDate;
        trip.EndDate = dto.EndDate;
        trip.ProjectId = dto.ProjectId;
        trip.BusinessEntityId = dto.BusinessEntityId;
        trip.Status = dto.Status;
        trip.Hotel = dto.Hotel;
        trip.Transport = dto.Transport;
        trip.Notes = dto.Notes;

        var existingMembers = await _context.TripMembers
            .Where(tm => tm.TripId == trip.Id)
            .ToListAsync();

        _context.TripMembers.RemoveRange(existingMembers);

        foreach (var userId in dto.TeamMemberIds.Distinct())
        {
            _context.TripMembers.Add(new TripMember
            {
                Id = Guid.NewGuid(),
                TripId = trip.Id,
                UserId = userId
            });
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.Id == id && t.IsActive);

        if (trip == null)
            return false;

        trip.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }
}