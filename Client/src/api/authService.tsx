import apiClient from "./apiClient";
import type { components } from "../types/api";
type LoginRequest = components["schemas"]["LoginRequest"];
import authenticatedApiClient from "./authenticatedApiClient";

const AuthService = {
  login: async (userData: LoginRequest) => {
    const response = await apiClient.post("/Auth/Login", userData);
    return response;
  },

  logout: async () => {
    const response = await authenticatedApiClient.post("/Auth/Logout");
    return response;
  },

  validate: async () => {
    const response = await apiClient.post("/Auth/Validate");
    return response;
  },
};

export default AuthService;
