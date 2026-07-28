import api from "./api";
import { City } from "@/types/city";
import { Contact, CreateCityPayload, CreateContactPayload } from "@/types/directory";

export const getCities = async (): Promise<City[]> => {
  const response = await api.get<City[]>("/directory/cities");
  return response.data;
};

export const createCity = async (payload: CreateCityPayload): Promise<City> => {
  const response = await api.post<City>("/directory/cities", payload);
  return response.data;
};

export const deleteCity = async (id: string): Promise<void> => {
  await api.delete(`/directory/cities/${id}`);
};

export const getContactsByCity = async (cityId: string): Promise<Contact[]> => {
  const response = await api.get<Contact[]>(`/directory/cities/${cityId}/contacts`);
  return response.data;
};

export const createContact = async (payload: CreateContactPayload): Promise<Contact> => {
  const response = await api.post<Contact>("/directory/contacts", payload);
  return response.data;
};

export const deleteContact = async (id: string): Promise<void> => {
  await api.delete(`/directory/contacts/${id}`);
};