export interface NextDeparture {
  city: string;
  startDate: string;
  endDate: string;
  daysUntil: number;
  status: string;
}

export interface DashboardData {
  upcomingTripsCount: number;
  nextDeparture: NextDeparture | null;
  totalTravelDaysThisYear: number;
  upcomingMeetingsCount: number;
  travelersThisWeekCount: number;
  tripsNeedingAttentionCount: number;
}