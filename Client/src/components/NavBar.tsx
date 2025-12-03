import { Link, useNavigate } from "react-router-dom";
import React from "react";
import useRoleChecker from "../hooks/useRoleChecker";
import useAuth from "../hooks/AuthContext";
import AuthService from "../api/authService";

const Navbar: React.FC = () => {
  const { isManager, isAdmin, isUser } = useRoleChecker();
  const { storeLogout, currentUser } = useAuth();
  const navigate = useNavigate();

  const logoutHandler = async () => {
    const response = await AuthService.logout();
    if (response.status === 200) {
      storeLogout();
      navigate("/Login");
    } else {
      console.error("Unable to logout");
    }
  };

  return (
    <nav className="row f-gap-4 align-center space-between">
      <div className="lg-font weight-600 color-primary-1">Skyline Software</div>
      <ul className="row f-gap-4 no-list">
        <li>
          <Link className="clickable color-primary-4 no-decoration" to="/">
            Home
          </Link>
        </li>

        {currentUser && (
          <li>
            <Link
              className="clickable color-primary-4 no-decoration"
              to={`/User/Edit/${currentUser.id}`}
            >
              My Account
            </Link>
          </li>
        )}

        {isAdmin() && (
          <>
            <li>
              <Link
                className="clickable color-primary-4 no-decoration"
                to="/AdminOnly"
              >
                Admin
              </Link>
            </li>
            <li>
              <Link
                className="clickable color-primary-4 no-decoration"
                to="/Users"
              >
                Users
              </Link>
            </li>
          </>
        )}

        {isManager() && (
          <li>
            <Link
              className="clickable color-primary-4 no-decoration"
              to="/ManagerOnly"
            >
              Manager
            </Link>
          </li>
        )}

        {isUser() && (
          <li>
            <Link
              className="clickable color-primary-4 no-decoration"
              to="/UserOnly"
            >
              User
            </Link>
          </li>
        )}
        <li>
          <div
            className="clickable color-primary-4 no-decoration"
            onClick={logoutHandler}
          >
            Logout
          </div>
        </li>
      </ul>
    </nav>
  );
};

export default Navbar;
