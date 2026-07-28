import api from "./api";
import { Hotel } from "@/types/hotel";

export const getHotelsByCity = async (cityId: string): Promise<Hotel[]> => {
  const response = await api.get<Hotel[]>(`/hotels/city/${cityId}`);
  return response.data;
};

export const createHotel = async (payload: { cityId: string; name: string; isCustom: boolean }): Promise<Hotel> => {
  const response = await api.post<Hotel>("/hotels", payload);
  return response.data;
};