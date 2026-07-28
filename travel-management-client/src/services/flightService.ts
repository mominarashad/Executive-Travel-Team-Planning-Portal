import api from "./api";
import { Flight, CreateFlightPayload } from "@/types/flight";

export const getFlights = async (): Promise<Flight[]> => {
  const response = await api.get<Flight[]>("/flights");
  return response.data;
};

export const createFlight = async (payload: CreateFlightPayload): Promise<Flight> => {
  const response = await api.post<Flight>("/flights", payload);
  return response.data;
};

export const updateFlight = async (id: string, payload: CreateFlightPayload): Promise<void> => {
  await api.put(`/flights/${id}`, payload);
};

export const deleteFlight = async (id: string): Promise<void> => {
  await api.delete(`/flights/${id}`);
};