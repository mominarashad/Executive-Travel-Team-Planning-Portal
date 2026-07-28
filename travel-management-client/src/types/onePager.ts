export interface OnePagerItineraryEntry {
  source: string;
  type: string;
  cityId: string | null;
  cityName: string;
  country: string;
  fromDate: string;
  toDate: string;
  tripId: string | null;
  notes: string | null;
}

export interface DaysByCountry {
  country: string;
  days: number;
}

export interface OnePagerMaterial {
  description: string;
  ownerName: string | null;
}

export interface OnePagerMeeting {
  tripId: string;
  tripCity: string;
  tripStartDate: string;
  tripEndDate: string;
  displayOrder: number;
  contactName: string;
  projectName: string;
  businessEntityName: string;
  status: string;
  priority: string;
  scheduledTime: string | null;
  agenda: string;
  team: string[];
  materials: OnePagerMaterial[];
}

export interface OnePager {
  userId: string;
  name: string;
  title: string;
  function: string;
  generatedAt: string;
  itinerary: OnePagerItineraryEntry[];
  daysByCountry: DaysByCountry[];
  totalDays: number;
  meetings: OnePagerMeeting[];
  flights: OnePagerFlight[];
}
export interface OnePagerFlight {
  tripCity: string;
  airline: string;
  flightNumber: string;
  departureTime: string;
  arrivalTime: string;
  departureAirport: string;
  arrivalAirport: string;
  aircraft: string;
  bookingReference: string;
}