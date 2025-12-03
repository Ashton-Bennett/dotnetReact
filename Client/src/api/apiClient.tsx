import axios, { type AxiosInstance } from "axios";

const apiClient: AxiosInstance = axios.create({
  baseURL: "http://localhost:5173/api",
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: true, // allows cookies to be sent/received automatically
});

apiClient.interceptors.request.use(async (config) => {
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response) {
      switch (error.response.status) {
        case 400:
          console.error("Bad Request", error.response.data);
          break;
        case 401:
          console.warn("Unauthorized - maybe redirect to login");
          break;
        case 500:
          console.error("Server error");
          break;
        default:
          console.error("Unexpected error", error.response.data);
      }
    } else {
      console.error("Network error or request cancelled");
    }
    return Promise.reject(error); // still reject to allow optional local handling
  }
);

export default apiClient;
