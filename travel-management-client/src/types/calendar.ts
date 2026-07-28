export interface CalendarEntry {
  source: string;
  type: string;
  cityId: string | null;
  cityName: string;
  fromDate: string;
  toDate: string;
  approvalStatus: string | null;
  tripId: string | null;
  notes: string | null;
}

export interface PersonCalendar {
  userId: string;
  name: string;
  title: string;
  function: string;
  entries: CalendarEntry[];
}