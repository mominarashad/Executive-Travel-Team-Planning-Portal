export interface Contact {
  id: string;
  name: string;
  organization: string;
  role: string;
  email: string;
  phone: string;
  sortOrder: number;
  cityId: string;
}

export interface CreateCityPayload {
  name: string;
  country: string;
}

export interface CreateContactPayload {
  name: string;
  organization: string;
  role: string;
  email: string;
  phone: string;
  sortOrder: number;
  cityId: string;
}