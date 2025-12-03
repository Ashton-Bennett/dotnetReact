import type { AxiosResponse } from "axios";

const isSuccessResponse = (response: AxiosResponse | null): boolean => {
  if (!response) {
    console.warn(
      "The response was null, unable to tell if it was successfull."
    );
    return false;
  }
  if (response.status >= 200 && response.status < 300) {
    return true;
  }

  return false;
};

export default isSuccessResponse;
