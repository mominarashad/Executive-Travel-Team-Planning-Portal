import api from "./api";
import { CreateMeetingPayload } from "@/types/meeting";

export const createMeeting = async (payload: CreateMeetingPayload) => {
  const response = await api.post("/meetings", payload);
  return response.data;
};

export const deleteMeeting = async (id: string): Promise<void> => {
  await api.delete(`/meetings/${id}`);
};