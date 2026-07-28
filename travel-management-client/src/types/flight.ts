export interface Flight {
  id: string;
  tripId: string;
  userId: string;
  airline: string;
  flightNumber: string;
  departureTime: string;
  arrivalTime: string;
  departureAirport: string;
  arrivalAirport: string;
  aircraft: string;
  bookingReference: string;
}

export interface CreateFlightPayload {
  tripId: string;
  userId: string;
  airline: string;
  flightNumber: string;
  departureTime: string;
  arrivalTime: string;
  departureAirport: string;
  arrivalAirport: string;
  aircraft: string;
  bookingReference: string;
}