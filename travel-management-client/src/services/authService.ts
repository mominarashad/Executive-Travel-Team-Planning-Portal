import api from "./api";
import { LoginResponse } from "@/types/auth";

export interface LoginRequest {
    email:string;
    password:string;
}


export const login = async (
  data: LoginRequest
): Promise<LoginResponse> => {
  const response = await api.post<LoginResponse>(
    "/auth/login",
    data
  );

  return response.data;
};