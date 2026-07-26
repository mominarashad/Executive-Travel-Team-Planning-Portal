namespace TravelManagement.API.Features.OnePager.DTOs;

public class OnePagerDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Function { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }

    public List<OnePagerItineraryEntryDto> Itinerary { get; set; } = new();
    public List<DaysByCountryDto> DaysByCountry { get; set; } = new();
    public int TotalDays { get; set; }

    public List<OnePagerMeetingDto> Meetings { get; set; } = new();
}