import api from "./api";
import { OnePager } from "@/types/onePager";

export const getOnePager = async (userId: string): Promise<OnePager> => {
  const response = await api.get<OnePager>(`/onepager/${userId}`);
  return response.data;
};
export const sendOnePagerEmail = async (userId: string, toEmail: string): Promise<{ message: string }> => {
  const response = await api.post<{ message: string }>(`/onepager/${userId}/send`, { toEmail });
  return response.data;
};