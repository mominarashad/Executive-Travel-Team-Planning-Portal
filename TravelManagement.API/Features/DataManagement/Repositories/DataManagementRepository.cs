using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Features.DataManagement.DTOs;
using TravelManagement.API.Features.DataManagement.Interfaces;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;
using TravelManagement.API.Infrastructure.Persistence.Entities.Flights;

namespace TravelManagement.API.Features.DataManagement.Repositories;

public class DataManagementRepository : IDataManagementRepository
{
    private readonly ApplicationDbContext _context;

    public DataManagementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ExportDataDto> ExportAsync()
    {
        return new ExportDataDto
        {
            ExportedAt = DateTime.UtcNow,

            Roles = await _context.Roles
                .Select(r => new RoleExportDto { Id = r.Id, Name = r.Name })
                .ToListAsync(),

            Users = await _context.Users
                .Select(u => new UserExportDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Title = u.Title,
                    Function = u.Function,
                    IsCeo = u.IsCeo,
                    RoleId = u.RoleId
                })
                .ToListAsync(),

            Cities = await _context.Cities
                .Select(c => new CityExportDto
                {
                    Id = c.Id, Name = c.Name, Country = c.Country, IsActive = c.IsActive
                })
                .ToListAsync(),

            Contacts = await _context.Contacts
                .Select(c => new ContactExportDto
                {
                    Id = c.Id, Name = c.Name, Organization = c.Organization, Role = c.Role,
                    Email = c.Email, Phone = c.Phone, SortOrder = c.SortOrder,
                    IsActive = c.IsActive, CityId = c.CityId
                })
                .ToListAsync(),

            Projects = await _context.Projects
                .Select(p => new ProjectExportDto
                {
                    Id = p.Id, Name = p.Name, IsSystem = p.IsSystem, IsActive = p.IsActive
                })
                .ToListAsync(),

            BusinessEntities = await _context.BusinessEntities
                .Select(e => new BusinessEntityExportDto
                {
                    Id = e.Id, Name = e.Name, IsSystem = e.IsSystem, IsActive = e.IsActive
                })
                .ToListAsync(),

            Hotels = await _context.Hotels
                .Select(h => new HotelExportDto
                {
                    Id = h.Id, CityId = h.CityId, Name = h.Name,
                    IsCustom = h.IsCustom, IsActive = h.IsActive
                })
                .ToListAsync(),

            Trips = await _context.Trips
                .Select(t => new TripExportDto
                {
                    Id = t.Id, DestinationCityId = t.DestinationCityId,
                    StartDate = t.StartDate, EndDate = t.EndDate,
                    ProjectId = t.ProjectId, BusinessEntityId = t.BusinessEntityId,
                    Status = t.Status, Hotel = t.Hotel, Transport = t.Transport,
                    FlightInfo = t.FlightInfo, Notes = t.Notes, IsActive = t.IsActive
                })
                .ToListAsync(),

            TripMembers = await _context.TripMembers
                .Select(tm => new TripMemberExportDto
                {
                    Id = tm.Id, TripId = tm.TripId, UserId = tm.UserId
                })
                .ToListAsync(),

            Meetings = await _context.Meetings
                .Select(m => new MeetingExportDto
                {
                    Id = m.Id, TripId = m.TripId, ContactId = m.ContactId,
                    DisplayOrder = m.DisplayOrder, Priority = m.Priority, Status = m.Status,
                    ScheduledTime = m.ScheduledTime, ProjectId = m.ProjectId,
                    BusinessEntityId = m.BusinessEntityId, Agenda = m.Agenda, IsActive = m.IsActive
                })
                .ToListAsync(),

            MeetingAttendees = await _context.MeetingAttendees
                .Select(a => new MeetingAttendeeExportDto
                {
                    Id = a.Id, MeetingId = a.MeetingId, UserId = a.UserId
                })
                .ToListAsync(),

            MeetingMaterials = await _context.MeetingMaterials
                .Select(mm => new MeetingMaterialExportDto
                {
                    Id = mm.Id, MeetingId = mm.MeetingId,
                    Description = mm.Description, OwnerId = mm.OwnerId
                })
                .ToListAsync(),

            Flights = await _context.Flights
                .Select(f => new FlightExportDto
                {
                    Id = f.Id, TripId = f.TripId, UserId = f.UserId,
                    Airline = f.Airline, FlightNumber = f.FlightNumber,
                    DepartureTime = f.DepartureTime, ArrivalTime = f.ArrivalTime,
                    DepartureAirport = f.DepartureAirport, ArrivalAirport = f.ArrivalAirport,
                    Aircraft = f.Aircraft, BookingReference = f.BookingReference,
                    IsActive = f.IsActive
                })
                .ToListAsync(),

            TeamPlanEntries = await _context.TeamPlanEntries
                .Select(e => new TeamPlanEntryExportDto
                {
                    Id = e.Id, UserId = e.UserId, CityId = e.CityId,
                    FromDate = e.FromDate, ToDate = e.ToDate, Type = e.Type,
                    ApprovalStatus = e.ApprovalStatus, Notes = e.Notes, IsActive = e.IsActive
                })
                .ToListAsync()
        };
    }

    public async Task ImportAsync(ExportDataDto data)
    {
        if (data == null)
            throw new InvalidOperationException("Import payload is empty.");

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // ---- Delete existing rows, children before parents ----
            // Note: Users and Roles are never touched on import.
            await _context.TeamPlanEntries.ExecuteDeleteAsync();
            await _context.Flights.ExecuteDeleteAsync();
            await _context.MeetingMaterials.ExecuteDeleteAsync();
            await _context.MeetingAttendees.ExecuteDeleteAsync();
            await _context.Meetings.ExecuteDeleteAsync();
            await _context.TripMembers.ExecuteDeleteAsync();
            await _context.Trips.ExecuteDeleteAsync();
            await _context.Hotels.ExecuteDeleteAsync();
            await _context.Contacts.ExecuteDeleteAsync();
            await _context.BusinessEntities.ExecuteDeleteAsync();
            await _context.Projects.ExecuteDeleteAsync();
            await _context.Cities.ExecuteDeleteAsync();

            // ---- Insert new rows, parents before children ----
            _context.Cities.AddRange(data.Cities.Select(c => new City
            {
                Id = c.Id, Name = c.Name, Country = c.Country, IsActive = c.IsActive
            }));
            await _context.SaveChangesAsync();

            _context.Projects.AddRange(data.Projects.Select(p => new Project
            {
                Id = p.Id, Name = p.Name, IsSystem = p.IsSystem, IsActive = p.IsActive
            }));
            await _context.SaveChangesAsync();

            _context.BusinessEntities.AddRange(data.BusinessEntities.Select(e => new BusinessEntity
            {
                Id = e.Id, Name = e.Name, IsSystem = e.IsSystem, IsActive = e.IsActive
            }));
            await _context.SaveChangesAsync();

            _context.Contacts.AddRange(data.Contacts.Select(c => new Contact
            {
                Id = c.Id, Name = c.Name, Organization = c.Organization, Role = c.Role,
                Email = c.Email, Phone = c.Phone, SortOrder = c.SortOrder,
                IsActive = c.IsActive, CityId = c.CityId
            }));
            await _context.SaveChangesAsync();

            _context.Hotels.AddRange(data.Hotels.Select(h => new Hotel
            {
                Id = h.Id, CityId = h.CityId, Name = h.Name,
                IsCustom = h.IsCustom, IsActive = h.IsActive
            }));
            await _context.SaveChangesAsync();

            _context.Trips.AddRange(data.Trips.Select(t => new Trip
            {
                Id = t.Id, DestinationCityId = t.DestinationCityId,
                StartDate = t.StartDate, EndDate = t.EndDate,
                ProjectId = t.ProjectId, BusinessEntityId = t.BusinessEntityId,
                Status = t.Status, Hotel = t.Hotel, Transport = t.Transport,
                FlightInfo = t.FlightInfo, Notes = t.Notes, IsActive = t.IsActive
            }));
            await _context.SaveChangesAsync();

            _context.TripMembers.AddRange(data.TripMembers.Select(tm => new TripMember
            {
                Id = tm.Id, TripId = tm.TripId, UserId = tm.UserId
            }));
            await _context.SaveChangesAsync();

            _context.Meetings.AddRange(data.Meetings.Select(m => new Meeting
            {
                Id = m.Id, TripId = m.TripId, ContactId = m.ContactId,
                DisplayOrder = m.DisplayOrder, Priority = m.Priority, Status = m.Status,
                ScheduledTime = m.ScheduledTime, ProjectId = m.ProjectId,
                BusinessEntityId = m.BusinessEntityId, Agenda = m.Agenda, IsActive = m.IsActive
            }));
            await _context.SaveChangesAsync();

            _context.MeetingAttendees.AddRange(data.MeetingAttendees.Select(a => new MeetingAttendee
            {
                Id = a.Id, MeetingId = a.MeetingId, UserId = a.UserId
            }));
            await _context.SaveChangesAsync();

            _context.MeetingMaterials.AddRange(data.MeetingMaterials.Select(mm => new MeetingMaterial
            {
                Id = mm.Id, MeetingId = mm.MeetingId,
                Description = mm.Description, OwnerId = mm.OwnerId
            }));
            await _context.SaveChangesAsync();

            _context.Flights.AddRange(data.Flights.Select(f => new Flight
            {
                Id = f.Id, TripId = f.TripId, UserId = f.UserId,
                Airline = f.Airline, FlightNumber = f.FlightNumber,
                DepartureTime = f.DepartureTime, ArrivalTime = f.ArrivalTime,
                DepartureAirport = f.DepartureAirport, ArrivalAirport = f.ArrivalAirport,
                Aircraft = f.Aircraft, BookingReference = f.BookingReference,
                IsActive = f.IsActive
            }));
            await _context.SaveChangesAsync();

            _context.TeamPlanEntries.AddRange(data.TeamPlanEntries.Select(e => new TeamPlanEntry
            {
                Id = e.Id, UserId = e.UserId, CityId = e.CityId,
                FromDate = e.FromDate, ToDate = e.ToDate, Type = e.Type,
                ApprovalStatus = e.ApprovalStatus, Notes = e.Notes, IsActive = e.IsActive
            }));
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException(
                $"Import failed and was rolled back. Reason: {ex.Message}");
        }
    }
}