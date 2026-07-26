using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Features.Calendar.DTOs;
using TravelManagement.API.Features.Calendar.Interfaces;
using TravelManagement.API.Infrastructure.Persistence;

namespace TravelManagement.API.Features.Calendar.Repositories;

public class CalendarRepository : ICalendarRepository
{
    private readonly ApplicationDbContext _context;

    public CalendarRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PersonCalendarDto>> GetCalendarAsync(
        DateOnly from, DateOnly to, List<Guid>? personIds)
    {
        var usersQuery = _context.Users.AsQueryable();
        if (personIds != null && personIds.Count > 0)
            usersQuery = usersQuery.Where(u => personIds.Contains(u.Id));

        var users = await usersQuery.ToListAsync();

        // Trip participation: TripMembers ∪ MeetingAttendees, per (UserId, TripId)
        var tripMemberPairs = await _context.TripMembers
            .Where(tm => tm.Trip.StartDate <= to && tm.Trip.EndDate >= from && tm.Trip.IsActive)
            .Select(tm => new { tm.UserId, tm.TripId })
            .ToListAsync();

        var meetingAttendeePairs = await _context.MeetingAttendees
            .Where(ma => ma.Meeting.Trip.StartDate <= to
                      && ma.Meeting.Trip.EndDate >= from
                      && ma.Meeting.Trip.IsActive)
            .Select(ma => new { ma.UserId, TripId = ma.Meeting.TripId })
            .ToListAsync();

        var tripParticipation = tripMemberPairs
            .Concat(meetingAttendeePairs)
            .Distinct()
            .ToList();

        var tripIds = tripParticipation.Select(p => p.TripId).Distinct().ToList();

        var trips = await _context.Trips
            .Where(t => tripIds.Contains(t.Id))
            .Include(t => t.DestinationCity)
            .ToDictionaryAsync(t => t.Id);

        var teamPlanEntries = await _context.TeamPlanEntries
            .Where(e => e.IsActive && e.FromDate <= to && e.ToDate >= from)
            .Include(e => e.City)
            .ToListAsync();

        var result = new List<PersonCalendarDto>();

        foreach (var user in users)
        {
            var entries = new List<CalendarEntryDto>();

            var userTripIds = tripParticipation
                .Where(p => p.UserId == user.Id)
                .Select(p => p.TripId)
                .Distinct();

            foreach (var tripId in userTripIds)
            {
                if (!trips.TryGetValue(tripId, out var trip)) continue;

                entries.Add(new CalendarEntryDto
                {
                    Source = "Trip",
                    Type = MapTripStatusToCalendarType(trip.Status),
                    CityId = trip.DestinationCityId,
                    CityName = trip.DestinationCity.Name,
                    FromDate = trip.StartDate,
                    ToDate = trip.EndDate,
                    TripId = trip.Id,
                    Notes = trip.Notes
                });
            }

            var userPlanEntries = teamPlanEntries.Where(e => e.UserId == user.Id);

            foreach (var entry in userPlanEntries)
            {
                bool isDuplicateOfTrip = entries.Any(e =>
                    e.Source == "Trip" &&
                    e.FromDate == entry.FromDate &&
                    e.ToDate == entry.ToDate &&
                    e.CityId == entry.CityId);

                if (isDuplicateOfTrip) continue;

                entries.Add(new CalendarEntryDto
                {
                    Source = "TeamPlan",
                    Type = entry.Type,
                    CityId = entry.CityId,
                    CityName = entry.City?.Name ?? "TBC",
                    FromDate = entry.FromDate,
                    ToDate = entry.ToDate,
                    ApprovalStatus = entry.Type == "Vacation" ? entry.ApprovalStatus : null,
                    Notes = entry.Notes
                });
            }

            result.Add(new PersonCalendarDto
            {
                UserId = user.Id,
                Name = user.Name,
                Title = user.Title,
                Function = user.Function,
                Entries = entries.OrderBy(e => e.FromDate).ToList()
            });
        }

        return result;
    }

    private static string MapTripStatusToCalendarType(string status) => status switch
    {
        "Confirmed" => "Trip",
        "Option" => "Option",
        "Tentative" => "Option",
        _ => "Option"
    };
}