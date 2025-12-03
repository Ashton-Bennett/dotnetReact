import AuthService from "../api/authService.tsx";
import { useNavigate } from "react-router-dom";
import useAuth from "./AuthContext.tsx";

const useLogout = () => {
  const navigate = useNavigate();
  const { storeLogout } = useAuth();

  const logoutHandler = async () => {
    const response = await AuthService.logout();
    if (response.status === 200) {
      storeLogout();
      navigate("/Login");
    } else {
      console.error("Unable to logout");
    }
  };
  return logoutHandler;
};

export default useLogout;
