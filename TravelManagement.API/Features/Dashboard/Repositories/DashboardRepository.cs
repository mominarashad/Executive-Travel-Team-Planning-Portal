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

        // ---- Upcoming trips count ----
        var upcomingTripsCount = await _context.Trips
            .CountAsync(t => t.IsActive && t.StartDate >= today);

        // ---- Next departure ----
        var nextTrip = await _context.Trips
            .Where(t => t.IsActive && t.StartDate >= today)
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
            .Where(t => t.IsActive && t.StartDate.Year == currentYear)
            .Select(t => new { t.StartDate, t.EndDate })
            .ToListAsync();

        var totalTravelDaysThisYear = thisYearTrips
            .Sum(t => t.EndDate.DayNumber - t.StartDate.DayNumber + 1);

        // ---- Upcoming meetings count ----
        // Meetings belonging to trips that haven't started yet.
        var upcomingMeetingsCount = await _context.Meetings
            .CountAsync(m => m.IsActive && m.Trip.IsActive && m.Trip.StartDate >= today);

        // ---- Travelers this week ----
        // Informational count (not a conflict check): anyone with any active
        // trip involvement (TripMember or MeetingAttendee) OR a TeamPlanEntry
        // of type Trip/Remote overlapping the next 7 days. Vacation is
        // intentionally excluded here since "traveling" != "on leave".
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
        // Upcoming (within the attention window) trips missing hotel or transport.
        var tripsNeedingAttentionCount = await _context.Trips
            .CountAsync(t => t.IsActive
                && t.StartDate >= today
                && t.StartDate <= attentionCutoff
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