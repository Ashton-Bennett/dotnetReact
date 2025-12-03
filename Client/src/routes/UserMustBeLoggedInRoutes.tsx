import { Navigate, Outlet } from "react-router-dom";
import useAuth from "../hooks/AuthContext"; // Example: your auth hook
import NavBarIncludedLayout from "../layouts/NavBarIncludedLayout";

const UserMustBeLoggedInRoutes = () => {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/Login" state={{ from: location.pathname }} replace />;
  }

  return (
    <NavBarIncludedLayout userLoggedIn={true}>
      <Outlet />
    </NavBarIncludedLayout>
  );
};

export default UserMustBeLoggedInRoutes;
