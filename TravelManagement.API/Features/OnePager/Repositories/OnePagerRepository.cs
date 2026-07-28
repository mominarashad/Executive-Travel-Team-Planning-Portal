using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Features.OnePager.DTOs;
using TravelManagement.API.Features.OnePager.Interfaces;
using TravelManagement.API.Infrastructure.Persistence;

namespace TravelManagement.API.Features.OnePager.Repositories;

public class OnePagerRepository : IOnePagerRepository
{
    private readonly ApplicationDbContext _context;

    public OnePagerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OnePagerDto?> GetOnePagerAsync(Guid userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return null;

        // ---- Trip participation: TripMembers ∪ MeetingAttendees, per TripId ----
        var tripMemberTripIds = await _context.TripMembers
            .Where(tm => tm.UserId == userId && tm.Trip.IsActive)
            .Select(tm => tm.TripId)
            .ToListAsync();

        var meetingAttendeeTripIds = await _context.MeetingAttendees
            .Where(a => a.UserId == userId && a.Meeting.Trip.IsActive)
            .Select(a => a.Meeting.TripId)
            .ToListAsync();

        var tripIds = tripMemberTripIds.Concat(meetingAttendeeTripIds).Distinct().ToList();

        var trips = await _context.Trips
            .Where(t => tripIds.Contains(t.Id))
            .Include(t => t.DestinationCity)
            .ToListAsync();

        // ---- TeamPlan entries for this person ----
        var planEntries = await _context.TeamPlanEntries
            .Where(e => e.IsActive && e.UserId == userId)
            .Include(e => e.City)
            .ToListAsync();

        // ---- Build merged, deduplicated itinerary ----
        var itinerary = new List<OnePagerItineraryEntryDto>();

        foreach (var trip in trips)
        {
            itinerary.Add(new OnePagerItineraryEntryDto
            {
                Source = "Trip",
                Type = trip.Status == "Confirmed" ? "Trip" : "Option",
                CityId = trip.DestinationCityId,
                CityName = trip.DestinationCity.Name,
                Country = trip.DestinationCity.Country,
                FromDate = trip.StartDate,
                ToDate = trip.EndDate,
                TripId = trip.Id,
                Notes = trip.Notes
            });
        }

        foreach (var entry in planEntries)
        {
            var isDuplicateOfTrip = itinerary.Any(i =>
                i.Source == "Trip" &&
                i.FromDate == entry.FromDate &&
                i.ToDate == entry.ToDate &&
                i.CityId == entry.CityId);

            if (isDuplicateOfTrip)
                continue;

            itinerary.Add(new OnePagerItineraryEntryDto
            {
                Source = "TeamPlan",
                Type = entry.Type,
                CityId = entry.CityId,
                CityName = entry.City?.Name ?? "TBC",
                Country = entry.City?.Country ?? string.Empty,
                FromDate = entry.FromDate,
                ToDate = entry.ToDate,
                Notes = entry.Notes
            });
        }

        itinerary = itinerary.OrderBy(i => i.FromDate).ToList();

        // ---- Days-by-country, computed from the merged itinerary ----
        var daysByCountry = itinerary
            .Where(i => !string.IsNullOrWhiteSpace(i.Country))
            .GroupBy(i => i.Country)
            .Select(g => new DaysByCountryDto
            {
                Country = g.Key,
                Days = g.Sum(i => i.ToDate.DayNumber - i.FromDate.DayNumber + 1)
            })
            .OrderBy(d => d.Country)
            .ToList();

        var totalDays = daysByCountry.Sum(d => d.Days);
        // ---- Flights on file for this person ----
        var flights = await _context.Flights
            .Where(f => f.IsActive && f.UserId == userId)
            .Include(f => f.Trip).ThenInclude(t => t.DestinationCity)
            .OrderBy(f => f.DepartureTime)
            .Select(f => new OnePagerFlightDto
            {
                TripCity = f.Trip.DestinationCity.Name,
                Airline = f.Airline,
                FlightNumber = f.FlightNumber,
                DepartureTime = f.DepartureTime,
                ArrivalTime = f.ArrivalTime,
                DepartureAirport = f.DepartureAirport,
                ArrivalAirport = f.ArrivalAirport,
                Aircraft = f.Aircraft,
                BookingReference = f.BookingReference
            })
            .ToListAsync();

        // ---- Meetings across all of this person's trips ----
        var meetings = await _context.Meetings


            .Where(m => m.IsActive && tripIds.Contains(m.TripId))
            .Include(m => m.Trip).ThenInclude(t => t.DestinationCity)
            .Include(m => m.Contact)
            .Include(m => m.Project)
            .Include(m => m.BusinessEntity)
            .Include(m => m.MeetingAttendees).ThenInclude(a => a.User)
            .Include(m => m.Materials).ThenInclude(mat => mat.Owner)
            .OrderBy(m => m.Trip.StartDate)
            .ThenBy(m => m.DisplayOrder)
            .Select(m => new OnePagerMeetingDto
            {
                TripId = m.TripId,
                TripCity = m.Trip.DestinationCity.Name,
                TripStartDate = m.Trip.StartDate,
                TripEndDate = m.Trip.EndDate,
                DisplayOrder = m.DisplayOrder,
                ContactName = m.Contact.Name,
                ProjectName = m.Project != null ? m.Project.Name : string.Empty,
                BusinessEntityName = m.BusinessEntity != null ? m.BusinessEntity.Name : string.Empty,
                Status = m.Status,
                Priority = m.Priority,
                ScheduledTime = m.ScheduledTime,
                Agenda = m.Agenda,
                Team = m.MeetingAttendees.Select(a => a.User.Name).ToList(),
                Materials = m.Materials.Select(mat => new OnePagerMaterialDto
                {
                    Description = mat.Description,
                    OwnerName = mat.Owner != null ? mat.Owner.Name : null
                }).ToList()
            })
            .ToListAsync();

        return new OnePagerDto
        {
            UserId = user.Id,
            Name = user.Name,
            Title = user.Title,
            Function = user.Function,
            GeneratedAt = DateTime.UtcNow,
            Itinerary = itinerary,
            DaysByCountry = daysByCountry,
            TotalDays = totalDays,
            Flights = flights,
            Meetings = meetings
        };
    }
}
