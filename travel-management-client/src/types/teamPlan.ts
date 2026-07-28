export interface TeamPlanEntry {
  id: string;
  userId: string;
  userName: string;
  cityId: string | null;
  cityName: string | null;
  fromDate: string;
  toDate: string;
  type: string;
  approvalStatus: string;
  notes: string;
}

export interface CreateTeamPlanPayload {
  userId: string;
  cityId: string | null;
  fromDate: string;
  toDate: string;
  type: string;
  approvalStatus: string;
  notes: string;
}

export interface BulkCreateTeamPlanPayload {
  userIds: string[];
  cityId: string | null;
  fromDate: string;
  toDate: string;
  type: string;
  approvalStatus: string;
  notes: string;
}