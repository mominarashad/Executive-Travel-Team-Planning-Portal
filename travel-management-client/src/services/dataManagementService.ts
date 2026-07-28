import api from "./api";

export const exportData = async (): Promise<Blob> => {
  const response = await api.get("/data/export", { responseType: "blob" });
  return response.data;
};

export const importData = async (data: unknown): Promise<{ message: string }> => {
  const response = await api.post<{ message: string }>("/data/import", data);
  return response.data;
};