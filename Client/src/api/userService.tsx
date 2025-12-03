import apiClient from "./apiClient";
import authenticatedApiClient from "./authenticatedApiClient";
import type { components } from "../types/api";
type User = components["schemas"]["User"];
type LoginRequest = components["schemas"]["LoginRequest"];

const UserService = {
  create: async (userData: LoginRequest) => {
    const response = await apiClient.post("/User/Create", userData);
    return response;
  },

  getById: async (id: string) => {
    const response = await authenticatedApiClient.get(`/User/${id}`);
    return response;
  },

  getAll: async () => {
    const response = await authenticatedApiClient.get("/User/GetAll");
    return response;
  },

  getRoles: async () => {
    const response = await authenticatedApiClient.get("/User/GetRoles");
    return response;
  },

  update: async (id: string, updatedUser: User) => {
    const response = await authenticatedApiClient.put(
      `/User/${id}`,
      updatedUser
    );
    return response;
  },

  delete: async (id: string) => {
    const response = await authenticatedApiClient.delete(`/User/${id}`);
    return response;
  },

  updatePassword: async (
    id: string,
    currentPassword: string,
    newPassword: string
  ) => {
    const response = await authenticatedApiClient.put(`/User/${id}/Password`, {
      currentPassword,
      newPassword,
    });
    return response;
  },
};

export default UserService;
