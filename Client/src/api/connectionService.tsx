import apiClient from "./apiClient";

const connectionService = {
  testBackend: async () => {
    const response = await apiClient.get("/Connection");
    return response;
  },
};

export default connectionService;
