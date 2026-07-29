import api from "./api";
import { Project } from "@/types/project";

export const getProjects = async (): Promise<Project[]> => {
  const response = await api.get<Project[]>("/projects");
  return response.data;
};

export const createProject = async (payload: { name: string }): Promise<Project> => {
  const response = await api.post<Project>("/projects", payload);
  return response.data;
};
