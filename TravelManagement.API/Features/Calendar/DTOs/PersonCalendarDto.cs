namespace TravelManagement.API.Features.Calendar.DTOs;

public class PersonCalendarDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Function { get; set; } = string.Empty;
    public List<CalendarEntryDto> Entries { get; set; } = new();
}