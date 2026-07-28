import api from "./api";
import { Trip, CreateTripPayload } from "@/types/trip";
import { TripDetail } from "@/types/trip";
import { BulkCreateTripPayload } from "@/types/trip";

export const getTrips = async (): Promise<Trip[]> => {
  const response = await api.get<Trip[]>("/trips");
  return response.data;
};

export const createTrip = async (payload: CreateTripPayload): Promise<Trip> => {
  const response = await api.post<Trip>("/trips", payload);
  return response.data;
};

export const deleteTrip = async (id: string): Promise<void> => {
  await api.delete(`/trips/${id}`);
};



export const getTripById = async (id: string): Promise<TripDetail> => {
  const response = await api.get<TripDetail>(`/trips/${id}`);
  return response.data;
};



export const bulkCreateTrips = async (payload: BulkCreateTripPayload): Promise<void> => {
  await api.post("/trips/bulk", payload);
};



export const updateTrip = async (id: string, payload: CreateTripPayload): Promise<void> => {
  await api.put(`/trips/${id}`, payload);
};