export interface TripTeamMember {
  id: string;
  name: string;
  title: string;
}

export interface Trip {
  id: string;
  destinationCityId: string;
  destinationCity: string;
  startDate: string;
  endDate: string;
  projectId: string | null;
  projectName: string;
  businessEntityId: string | null;
  businessEntityName: string;
  status: string;
  hotel: string;
  transport: string;
  flightInfo: string;
  notes: string;
  teamMemberIds: string[];
  teamMembers: TripTeamMember[];
}

export interface CreateTripPayload {
  destinationCityId: string;
  startDate: string;
  endDate: string;
  projectId: string | null;
  businessEntityId: string | null;
  status: string;
  hotel: string;
  transport: string;
  notes: string;
  teamMemberIds: string[];
}

export interface TripMeetingAttendee {
  id: string;
  name: string;
}

export interface TripMeetingMaterial {
  id: string;
  description: string;
  ownerName: string | null;
}

export interface TripMeeting {
  id: string;
  contactName: string;
  displayOrder: number;
  priority: string;
  status: string;
  scheduledTime: string | null;
  projectName: string;
  businessEntityName: string;
  agenda: string;
  attendees: TripMeetingAttendee[];
  materials: TripMeetingMaterial[];
}

export interface TripDetail extends Trip {
  meetings: TripMeeting[];
}

export interface BulkTripLeg {
  destinationCityId: string;
  startDate: string;
  endDate: string;
  projectId: string | null;
  businessEntityId: string | null;
  status: string;
}

export interface BulkCreateTripPayload {
  trips: BulkTripLeg[];
}