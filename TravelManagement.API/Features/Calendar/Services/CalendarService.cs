using TravelManagement.API.Features.Calendar.DTOs;
using TravelManagement.API.Features.Calendar.Interfaces;

namespace TravelManagement.API.Features.Calendar.Services;

public class CalendarService : ICalendarService
{
    private readonly ICalendarRepository _repository;

    public CalendarService(ICalendarRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PersonCalendarDto>> GetCalendarAsync(
        DateOnly from, DateOnly to, List<Guid>? personIds) =>
        await _repository.GetCalendarAsync(from, to, personIds);
}