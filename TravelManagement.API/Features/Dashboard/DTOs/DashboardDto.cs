namespace TravelManagement.API.Features.Dashboard.DTOs;

public class DashboardDto
{
    public int UpcomingTripsCount { get; set; }
    public NextDepartureDto? NextDeparture { get; set; }
    public int TotalTravelDaysThisYear { get; set; }
    public int UpcomingMeetingsCount { get; set; }
    public int TravelersThisWeekCount { get; set; }
    public int TripsNeedingAttentionCount { get; set; }
}