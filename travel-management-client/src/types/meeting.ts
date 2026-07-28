export interface CreateMeetingMaterialPayload {
  description: string;
  ownerId: string | null;
}

export interface CreateMeetingPayload {
  tripId: string;
  contactId: string;
  displayOrder: number;
  priority: string;
  status: string;
  scheduledTime: string | null;
  projectId: string | null;
  businessEntityId: string | null;
  agenda: string;
  attendeeIds: string[];
  materials: CreateMeetingMaterialPayload[];
}