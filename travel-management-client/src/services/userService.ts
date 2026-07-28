import api from "./api";
import { AppUser } from "@/types/user";

export const getUsers = async (): Promise<AppUser[]> => {
  const response = await api.get<AppUser[]>("/users");
  return response.data;
};