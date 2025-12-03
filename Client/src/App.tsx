import { useState, useEffect } from "react";
import AuthService from "./api/authService.tsx";
import isSuccessResponse from "./utilities/isSuccessResponse.tsx";
import useAuth from "./hooks/AuthContext.tsx";
import "./styles/site.css";
import { Routes, Route, useNavigate, useLocation } from "react-router-dom";
import Home from "./Pages/Home.tsx";
import Login from "./Pages/Login.tsx";
import UserMustBeLoggedInRoutes from "./routes/UserMustBeLoggedInRoutes.tsx";
import AboutPage from "./Pages/AboutPage.tsx";
import AdminOnly from "./Pages/AdminOnly.tsx";
import ManagerOnly from "./Pages/ManagerOnly.tsx";
import UserOnly from "./Pages/UserOnly.tsx";
import Footer from "./components/Footer.tsx";
import Users from "./Pages/User/Users.tsx";
import EditUser from "./Pages/User/EditUser.tsx";

const App = () => {
  const navigate = useNavigate();
  const { storeLogin } = useAuth();
  const [loading, setLoading] = useState(true);
  const location = useLocation();
  const from = location.pathname || "/";

  useEffect(() => {
    const validateOrRequireLogin = async () => {
      try {
        const response = await AuthService.validate();
        if (isSuccessResponse(response)) {
          storeLogin(response.data.accessToken, response.data);
          navigate(from, { replace: true });
          setLoading(false);
        } else {
          setLoading(false);
        }
      } catch {
        setLoading(false);
      }
    };

    validateOrRequireLogin();
  }, []);

  return (
    <>
      {loading ? (
        <div className="color-shade-13 lg-font row center full-height full-width">
          Loading...
        </div>
      ) : (
        <Routes>
          <Route path="/Login" element={<Login />} />

          {/* Protected Routes */}
          <Route element={<UserMustBeLoggedInRoutes />}>
            <Route path="/" element={<Home />} />
            <Route path="/AdminOnly" element={<AdminOnly />} />
            <Route path="/ManagerOnly" element={<ManagerOnly />} />
            <Route path="/UserOnly" element={<UserOnly />} />
            <Route path="/Users" element={<Users />} />
            <Route path="/User/Edit/:id" element={<EditUser />} />
          </Route>

          {/* Unprotected Routes */}
          <Route path="/AboutUs" element={<AboutPage />} />

          {/* 404 fallback */}
          <Route path="*" element={<h2>Page not found</h2>} />
        </Routes>
      )}

      <Footer />
    </>
  );
};

export default App;
