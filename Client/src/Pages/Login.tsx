import { useState, useEffect } from "react";
import UserService from "../api/userService";
import connectionService from "../api/connectionService";
import AuthService from "../api/authService";
import isSuccessResponse from "../utilities/isSuccessResponse";
import useAuth from "../hooks/AuthContext";
import { useNavigate, useLocation } from "react-router-dom";

const Login = () => {
  const [confirmBackendMessage, setConfirmBackendMessage] =
    useState<string>("Not Connected.");
  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [error, setError] = useState<string>("");
  const [isCreateForm, setIsCreateForm] = useState<boolean>(false);
  const { storeLogin } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const state = location.state as { from?: string } | null;

  let from = state?.from || "/";
  if (from.toLowerCase() === "/login") {
    from = "/";
  }

  useEffect(() => {
    const getMessage = async () => {
      try {
        const res = await connectionService.testBackend();
        setConfirmBackendMessage(res.data.message);
      } catch (err) {
        console.error("Fetch error:", err);
        setConfirmBackendMessage("Unable to connect");
      }
    };

    getMessage();
  }, []);

  const createUser = async () => {
    try {
      const response = await UserService.create({ email, password });
      if (isSuccessResponse(response)) {
        setIsCreateForm(false);
        loginHandler();
      } else {
        setError("Sorry unable to create user. Please try again.");
      }
    } catch (err) {
      setError(
        // @ts-ignore
        err.response?.data?.message || "An unexpected error occurred."
      );
    }
  };

  const loginHandler = async () => {
    try {
      const response = await AuthService.login({ email, password });
      if (isSuccessResponse(response)) {
        storeLogin(response.data.accessToken, response.data);
        navigate(from, { replace: true });
      } else {
        setError(
          "Sorry unable to login. Please check your email and password then try again."
        );
      }
    } catch (err) {
      setError(
        // @ts-ignore
        err.response?.data?.message || "An unexpected error occurred."
      );
    }
  };

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!email || !password) {
      setError("Please enter both email and password.");
      return;
    }

    if (isCreateForm) {
      createUser();
    } else {
      loginHandler();
    }

    setError("");
  };

  const toggleCreateAccount = () => {
    setIsCreateForm(!isCreateForm);
  };

  return (
    <section className="column center full-view-height">
      <header>Backend is: {confirmBackendMessage}</header>
      <form
        onSubmit={handleSubmit}
        className="column f-gap-4 white-border p-4 radius center mx-wd-350"
      >
        <h1 className="lg-font">{isCreateForm ? "Create Account" : "Login"}</h1>

        {error && <p style={{ color: "red" }}>{error}</p>}

        <div className="column">
          <label className="sm-font">Email</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </div>

        <div className="column">
          <label className="sm-font">Password</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>
        <button type="button" onClick={toggleCreateAccount}>
          {isCreateForm ? "Go Back" : "New Here? Create Account"}
        </button>
        <button type="submit"> {isCreateForm ? "Create" : "Login"}</button>
      </form>
    </section>
  );
};

export default Login;
