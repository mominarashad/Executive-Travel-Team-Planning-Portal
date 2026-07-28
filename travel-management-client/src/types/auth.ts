export interface User {
  id: string;
  name: string;
  email: string;
  role: string;
  isCeo: boolean;
}

export interface LoginResponse {
  token: string;
  user: User;
}