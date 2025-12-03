import axios, { type AxiosInstance } from "axios";
import tokenStore from "../utilities/tokenStore";
import isTokenExpired from "../utilities/isTokenExpired";
import isSuccessResponse from "../utilities/isSuccessResponse";

const authenticatedApiClient: AxiosInstance = axios.create({
  baseURL: "http://localhost:5173/api",
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: true, // allows cookies to be sent/received automatically
});

authenticatedApiClient.interceptors.request.use(async (config) => {
  let token = tokenStore.get();

  if (token && isTokenExpired(token)) {
    // Call refresh endpoint
    const response = await axios.post(
      "/api/Auth/Refresh",
      {},
      { withCredentials: true }
    );

    if (isSuccessResponse(response)) {
      token = response.data.accessToken;
      tokenStore.set(token);
    } else {
      console.error("unable to refresh token");
    }
  }

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

authenticatedApiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response) {
      switch (error.response.status) {
        case 400:
          console.error("Bad Request", error.response.data);
          break;
        case 401:
          window.location.href = "/";
          console.warn("Unauthorized - maybe redirecting to /");
          break;
        case 403:
          console.warn("Forbidden - you dont have access");
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

export default authenticatedApiClient;
