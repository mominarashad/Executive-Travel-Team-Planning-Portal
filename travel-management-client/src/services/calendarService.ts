import api from "./api";
import { PersonCalendar } from "@/types/calendar";

export const getCalendar = async (
  from: string,
  to: string,
  personIds?: string[]
): Promise<PersonCalendar[]> => {
  const params = new URLSearchParams();
  params.append("from", from);
  params.append("to", to);
  personIds?.forEach((id) => params.append("personIds", id));

  const response = await api.get<PersonCalendar[]>(`/calendar?${params.toString()}`);
  return response.data;
};