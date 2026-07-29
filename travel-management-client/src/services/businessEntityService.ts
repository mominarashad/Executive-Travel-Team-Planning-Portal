import api from "./api";
import { BusinessEntity } from "@/types/businessEntity";

export const getBusinessEntities = async (): Promise<BusinessEntity[]> => {
  const response = await api.get<BusinessEntity[]>("/entities");
  return response.data;
};

export const createBusinessEntity = async (payload: { name: string }): Promise<BusinessEntity> => {
  const response = await api.post<BusinessEntity>("/entities", payload);
  return response.data;
};
