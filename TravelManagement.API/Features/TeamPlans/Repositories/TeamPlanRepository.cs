using Microsoft.EntityFrameworkCore;
using TravelManagement.API.Features.TeamPlans.DTOs;
using TravelManagement.API.Features.TeamPlans.Interfaces;
using TravelManagement.API.Infrastructure.Persistence;
using TravelManagement.API.Infrastructure.Persistence.Entities;
using TravelManagement.API.Features.Email.Interfaces;
namespace TravelManagement.API.Features.TeamPlans.Repositories;

public class TeamPlanRepository : ITeamPlanRepository
{
    private readonly ApplicationDbContext _context;

    private static readonly string[] ValidTypes = { "Trip", "Option", "Vacation", "Remote" };
    private static readonly string[] ValidApprovalStatuses = { "", "Pending", "Approved", "Rejected" };

    private readonly IEmailService _emailService;

public TeamPlanRepository(ApplicationDbContext context, IEmailService emailService)
{
    _context = context;
    _emailService = emailService;
}

    // ---------- shared validation ----------

    private static string NormalizeType(string type)
    {
        var match = ValidTypes.FirstOrDefault(t => t.Equals(type?.Trim(), StringComparison.OrdinalIgnoreCase));
        if (match == null)
            throw new InvalidOperationException(
                $"Invalid type '{type}'. Must be one of: {string.Join(", ", ValidTypes)}.");
        return match;
    }

    private static string NormalizeApprovalStatus(string? status)
    {
        var value = status?.Trim() ?? string.Empty;
        var match = ValidApprovalStatuses.FirstOrDefault(s => s.Equals(value, StringComparison.OrdinalIgnoreCase));
        if (match == null)
            throw new InvalidOperationException(
                $"Invalid approval status '{status}'. Must be one of: Pending, Approved, Rejected.");
        return match;
    }

    private async Task ValidateReferencesAsync(Guid userId, Guid? cityId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
            throw new InvalidOperationException("User not found.");

        if (cityId.HasValue)
        {
            var cityExists = await _context.Cities.AnyAsync(c => c.Id == cityId.Value && c.IsActive);
            if (!cityExists)
                throw new InvalidOperationException("City not found.");
        }
    }

    private static bool CountsAsConfirmed(string type, string approvalStatus) =>
        type == "Trip" || type == "Remote" || (type == "Vacation" && approvalStatus == "Approved");

    private async Task EnsureNoDoubleBookingAsync(
        Guid userId,
        DateOnly from,
        DateOnly to,
        Guid? excludeEntryId)
    {
        // Conflict against an existing confirmed Trip
        var tripConflict = await _context.Trips
            .Where(t => t.IsActive
                && t.Status == "Confirmed"
                && t.StartDate <= to && from <= t.EndDate
                && (t.TripMembers.Any(tm => tm.UserId == userId)
                    || t.Meetings.Any(m => m.MeetingAttendees.Any(a => a.UserId == userId))))
            .Select(t => new { t.DestinationCity.Name, t.StartDate, t.EndDate })
            .FirstOrDefaultAsync();

        if (tripConflict != null)
            throw new InvalidOperationException(
                $"Team member is already confirmed on a trip to {tripConflict.Name} " +
                $"({tripConflict.StartDate:yyyy-MM-dd} to {tripConflict.EndDate:yyyy-MM-dd}) " +
                "that overlaps these dates.");

        // Conflict against another confirmed TeamPlanEntry
        var candidates = await _context.TeamPlanEntries
            .Where(e => e.IsActive
                && e.UserId == userId
                && (excludeEntryId == null || e.Id != excludeEntryId)
                && e.FromDate <= to && from <= e.ToDate)
            .ToListAsync();

        var planConflict = candidates.FirstOrDefault(e => CountsAsConfirmed(e.Type, e.ApprovalStatus));

        if (planConflict != null)
            throw new InvalidOperationException(
                $"Team member already has a {planConflict.Type.ToLower()} entry " +
                $"({planConflict.FromDate:yyyy-MM-dd} to {planConflict.ToDate:yyyy-MM-dd}) " +
                "that overlaps these dates.");
    }

    private async Task<(string type, string approvalStatus)> ValidateAndCheckAsync(
        Guid userId,
        Guid? cityId,
        DateOnly from,
        DateOnly to,
        string type,
        string? approvalStatus,
        Guid? excludeEntryId)
    {
        if (to < from)
            throw new InvalidOperationException("To date cannot be earlier than From date.");

        var normalizedType = NormalizeType(type);
        var normalizedApproval = NormalizeApprovalStatus(approvalStatus);

        await ValidateReferencesAsync(userId, cityId);
        await EnsureNoDoubleBookingAsync(userId, from, to, excludeEntryId);

        return (normalizedType, normalizedApproval);
    }

    // ---------- reads (unchanged) ----------

    public async Task<IEnumerable<TeamPlanDto>> GetAllAsync()
    {
        return await _context.TeamPlanEntries
            .Where(tp => tp.IsActive)
            .OrderBy(tp => tp.FromDate)
            .Select(tp => new TeamPlanDto
            {
                Id = tp.Id,
                UserId = tp.UserId,
                UserName = tp.User.Name,
                CityId = tp.CityId,
                CityName = tp.City != null ? tp.City.Name : null,
                FromDate = tp.FromDate,
                ToDate = tp.ToDate,
                Type = tp.Type,
                ApprovalStatus = tp.ApprovalStatus,
                Notes = tp.Notes
            })
            .ToListAsync();
    }

    public async Task<TeamPlanDto?> GetByIdAsync(Guid id)
    {
        return await _context.TeamPlanEntries
            .Where(tp => tp.Id == id && tp.IsActive)
            .Select(tp => new TeamPlanDto
            {
                Id = tp.Id,
                UserId = tp.UserId,
                UserName = tp.User.Name,
                CityId = tp.CityId,
                CityName = tp.City != null ? tp.City.Name : null,
                FromDate = tp.FromDate,
                ToDate = tp.ToDate,
                Type = tp.Type,
                ApprovalStatus = tp.ApprovalStatus,
                Notes = tp.Notes
            })
            .FirstOrDefaultAsync();
    }

    // ---------- writes (validated) ----------

    public async Task<TeamPlanDto> CreateAsync(CreateTeamPlanDto dto)
    {
        var (type, approval) = await ValidateAndCheckAsync(
            dto.UserId, dto.CityId, dto.FromDate, dto.ToDate,
            dto.Type, dto.ApprovalStatus, excludeEntryId: null);

        var entry = new TeamPlanEntry
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            CityId = dto.CityId,
            FromDate = dto.FromDate,
            ToDate = dto.ToDate,
            Type = type,
            ApprovalStatus = approval,
            Notes = dto.Notes,
            IsActive = true
        };

        _context.TeamPlanEntries.Add(entry);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(entry.Id)
            ?? throw new Exception("Team Plan could not be loaded.");
    }
    public async Task BulkCreateAsync(BulkCreateTeamPlanDto dto)
{
    if (dto.UserIds == null || dto.UserIds.Count == 0)
        throw new InvalidOperationException("At least one user is required.");

    if (dto.FromDate == null || dto.ToDate == null)
        throw new InvalidOperationException("From and To dates are required.");

    var from = dto.FromDate.Value;
    var to = dto.ToDate.Value;

    foreach (var userId in dto.UserIds.Distinct())
    {
        var (type, approval) = await ValidateAndCheckAsync(
            userId, dto.CityId, from, to,
            dto.Type, dto.ApprovalStatus, excludeEntryId: null);

        var entry = new TeamPlanEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CityId = dto.CityId,
            FromDate = from,
            ToDate = to,
            Type = type,
            ApprovalStatus = approval,
            Notes = dto.Notes ?? string.Empty,
            IsActive = true
        };

        _context.TeamPlanEntries.Add(entry);

        // Saved per-entry so subsequent iterations in this same batch
        // see prior entries when checking for conflicts.
        await _context.SaveChangesAsync();
    }
}

    public async Task<bool> UpdateAsync(Guid id, UpdateTeamPlanDto dto)
{
    var entry = await _context.TeamPlanEntries
        .FirstOrDefaultAsync(tp => tp.Id == id && tp.IsActive);

    if (entry == null)
        return false;

    var previousApprovalStatus = entry.ApprovalStatus;

    var (type, approval) = await ValidateAndCheckAsync(
        dto.UserId, dto.CityId, dto.FromDate, dto.ToDate,
        dto.Type, dto.ApprovalStatus, excludeEntryId: entry.Id);

    entry.UserId = dto.UserId;
    entry.CityId = dto.CityId;
    entry.FromDate = dto.FromDate;
    entry.ToDate = dto.ToDate;
    entry.Type = type;
    entry.ApprovalStatus = approval;
    entry.Notes = dto.Notes;

    await _context.SaveChangesAsync();

    // Notify on a genuine status change for a Vacation entry only
    // (Approved/Rejected are the only decisions worth notifying about).
    if (type == "Vacation"
        && previousApprovalStatus != approval
        && (approval == "Approved" || approval == "Rejected"))
    {
        var user = await _context.Users.FindAsync(entry.UserId);
        if (user != null && !string.IsNullOrWhiteSpace(user.Email))
        {
            var subject = $"Vacation Request {approval} — {entry.FromDate:MMM d} to {entry.ToDate:MMM d}";
            var body = $@"
                <h2>Vacation Request Update</h2>
                <p>Hi {user.Name},</p>
                <p>Your vacation request from <strong>{entry.FromDate:MMM d, yyyy}</strong> to
                <strong>{entry.ToDate:MMM d, yyyy}</strong> has been
                <strong style='color:{(approval == "Approved" ? "green" : "red")}'>{approval}</strong>.</p>
                {(string.IsNullOrWhiteSpace(entry.Notes) ? "" : $"<p>Notes: {entry.Notes}</p>")}
            ";

            // Don't let an email failure roll back the actual approval decision —
            // log-and-continue rather than throw.
            try
            {
                await _emailService.SendEmailAsync(user.Email, subject, body);
            }
            catch
            {
                // Intentionally swallowed: the approval itself already succeeded and
                // was saved; a notification failure shouldn't undo a real decision.
            }
        }
    }

    return true;
}

    public async Task<IEnumerable<TeamPlanSummaryDto>> GetSummaryAsync(Guid userId)
    {
        var plans = await _context.TeamPlanEntries
            .Include(tp => tp.City)
            .Where(tp => tp.UserId == userId && tp.IsActive && tp.City != null)
            .ToListAsync();

        var summary = plans
            .GroupBy(tp => tp.City!.Country)
            .Select(g => new TeamPlanSummaryDto
            {
                Country = g.Key,
                Days = g.Sum(tp => tp.ToDate.DayNumber - tp.FromDate.DayNumber + 1)
            })
            .OrderBy(x => x.Country)
            .ToList();

        return summary;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entry = await _context.TeamPlanEntries
            .FirstOrDefaultAsync(tp => tp.Id == id && tp.IsActive);

        if (entry == null)
            return false;

        entry.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }
}
