using TravelManagement.API.Features.Calendar.DTOs;

namespace TravelManagement.API.Features.Calendar.Interfaces;

public interface ICalendarService
{
    Task<IEnumerable<PersonCalendarDto>> GetCalendarAsync(DateOnly from, DateOnly to, List<Guid>? personIds);
}