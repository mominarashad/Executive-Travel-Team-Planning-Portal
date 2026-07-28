import api from "./api";
import {
  TeamPlanEntry,
  CreateTeamPlanPayload,
  BulkCreateTeamPlanPayload,
} from "@/types/teamPlan";

export const getTeamPlans = async (): Promise<TeamPlanEntry[]> => {
  const response = await api.get<TeamPlanEntry[]>("/TeamPlans");
  return response.data;
};

export const createTeamPlan = async (payload: CreateTeamPlanPayload): Promise<TeamPlanEntry> => {
  const response = await api.post<TeamPlanEntry>("/TeamPlans", payload);
  return response.data;
};

export const updateTeamPlan = async (id: string, payload: CreateTeamPlanPayload): Promise<void> => {
  await api.put(`/TeamPlans/${id}`, payload);
};

export const deleteTeamPlan = async (id: string): Promise<void> => {
  await api.delete(`/TeamPlans/${id}`);
};

export const bulkCreateTeamPlans = async (payload: BulkCreateTeamPlanPayload): Promise<void> => {
  await api.post("/TeamPlans/bulk", payload);
};