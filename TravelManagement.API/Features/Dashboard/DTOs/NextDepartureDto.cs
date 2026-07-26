namespace TravelManagement.API.Features.Dashboard.DTOs;

public class NextDepartureDto
{
    public string City { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int DaysUntil { get; set; }
    public string Status { get; set; } = string.Empty;
}