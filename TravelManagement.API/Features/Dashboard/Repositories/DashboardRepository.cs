using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Features.Dashboard.DTOs;
using TravelManagement.API.Features.Dashboard.Interfaces;
using TravelManagement.API.Infrastructure.Persistence;

namespace TravelManagement.API.Features.Dashboard.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;
    private const int ATTENTION_WINDOW_DAYS = 14;
    private const int WEEK_WINDOW_DAYS = 6; // today + 6 = 7-day window inclusive

    public DashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var currentYear = today.Year;
        var weekEnd = today.AddDays(WEEK_WINDOW_DAYS);
        var attentionCutoff = today.AddDays(ATTENTION_WINDOW_DAYS);

        // ---- Resolve the CEO ----
        // Dashboard is scoped to the CEO specifically: only trips/meetings where
        // the CEO is a TripMember or MeetingAttendee count. This excludes
        // delegation trips created without the CEO (per the "soft default,
        // never enforced" team-member rule on trip creation).
        var ceoId = await _context.Users
            .Where(u => u.IsCeo)
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync();

        // If no CEO is configured, every metric below returns zeroed/empty
        // rather than silently falling back to company-wide totals.
        if (ceoId == null)
        {
            return new DashboardDto
            {
                UpcomingTripsCount = 0,
                NextDeparture = null,
                TotalTravelDaysThisYear = 0,
                UpcomingMeetingsCount = 0,
                TravelersThisWeekCount = 0,
                TripsNeedingAttentionCount = 0
            };
        }

        // A trip "involves the CEO" if he's a TripMember OR attends any of its meetings.
        var ceoTripIds = await _context.TripMembers
            .Where(tm => tm.UserId == ceoId)
            .Select(tm => tm.TripId)
            .Union(
                _context.MeetingAttendees
                    .Where(a => a.UserId == ceoId)
                    .Select(a => a.Meeting.TripId)
            )
            .Distinct()
            .ToListAsync();

        // ---- Upcoming trips count ----
        var upcomingTripsCount = await _context.Trips
            .CountAsync(t => t.IsActive && t.StartDate >= today && ceoTripIds.Contains(t.Id));

        // ---- Next departure ----
        var nextTrip = await _context.Trips
            .Where(t => t.IsActive && t.StartDate >= today && ceoTripIds.Contains(t.Id))
            .OrderBy(t => t.StartDate)
            .Select(t => new
            {
                CityName = t.DestinationCity.Name,
                t.StartDate,
                t.EndDate,
                t.Status
            })
            .FirstOrDefaultAsync();

        NextDepartureDto? nextDeparture = nextTrip == null ? null : new NextDepartureDto
        {
            City = nextTrip.CityName,
            StartDate = nextTrip.StartDate,
            EndDate = nextTrip.EndDate,
            DaysUntil = nextTrip.StartDate.DayNumber - today.DayNumber,
            Status = nextTrip.Status
        };

        // ---- Total travel days this year ----
        // Assumption: trips are attributed to the year of their StartDate.
        // A trip spanning a year boundary (e.g. Dec 30 - Jan 2) counts fully
        // toward the year it starts in, not split across years.
        var thisYearTrips = await _context.Trips
            .Where(t => t.IsActive && t.StartDate.Year == currentYear && ceoTripIds.Contains(t.Id))
            .Select(t => new { t.StartDate, t.EndDate })
            .ToListAsync();

        var totalTravelDaysThisYear = thisYearTrips
            .Sum(t => t.EndDate.DayNumber - t.StartDate.DayNumber + 1);

        // ---- Upcoming meetings count ----
        // Meetings belonging to CEO trips that haven't started yet.
        var upcomingMeetingsCount = await _context.Meetings
            .CountAsync(m => m.IsActive && m.Trip.IsActive && m.Trip.StartDate >= today
                && ceoTripIds.Contains(m.TripId));

        // ---- Travelers this week ----
        // Kept company-wide (not CEO-scoped) — this metric answers "who is
        // traveling this week" for planning purposes, not "who is with the CEO."
        var tripTravelers = await _context.TripMembers
            .Where(tm => tm.Trip.IsActive
                && tm.Trip.StartDate <= weekEnd
                && today <= tm.Trip.EndDate)
            .Select(tm => tm.UserId)
            .ToListAsync();

        var meetingTravelers = await _context.MeetingAttendees
            .Where(a => a.Meeting.Trip.IsActive
                && a.Meeting.Trip.StartDate <= weekEnd
                && today <= a.Meeting.Trip.EndDate)
            .Select(a => a.UserId)
            .ToListAsync();

        var planTravelers = await _context.TeamPlanEntries
            .Where(e => e.IsActive
                && (e.Type == "Trip" || e.Type == "Remote")
                && e.FromDate <= weekEnd
                && today <= e.ToDate)
            .Select(e => e.UserId)
            .ToListAsync();

        var travelersThisWeekCount = tripTravelers
            .Concat(meetingTravelers)
            .Concat(planTravelers)
            .Distinct()
            .Count();

        // ---- Trips needing attention ----
        // Upcoming (within the attention window) CEO trips missing hotel or transport.
        var tripsNeedingAttentionCount = await _context.Trips
            .CountAsync(t => t.IsActive
                && t.StartDate >= today
                && t.StartDate <= attentionCutoff
                && ceoTripIds.Contains(t.Id)
                && (string.IsNullOrWhiteSpace(t.Hotel) || string.IsNullOrWhiteSpace(t.Transport)));

        return new DashboardDto
        {
            UpcomingTripsCount = upcomingTripsCount,
            NextDeparture = nextDeparture,
            TotalTravelDaysThisYear = totalTravelDaysThisYear,
            UpcomingMeetingsCount = upcomingMeetingsCount,
            TravelersThisWeekCount = travelersThisWeekCount,
            TripsNeedingAttentionCount = tripsNeedingAttentionCount
        };
    }
}
