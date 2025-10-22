import { useState, useEffect } from "react";
import React from "react";

const Login = () => {
  const [confirmBackendMessage, setConfirmBackendMessage] =
    useState<string>("Not Connected.");
  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [error, setError] = useState<string>("");
  const [isCreateForm, setIsCreateForm] = useState<boolean>(false);

  useEffect(() => {
    const fetchMessage = async () => {
      try {
        const res = await fetch("/api/greetings");
        if (!res.ok) throw new Error("Network response was not ok");
        const data = await res.json();
        setConfirmBackendMessage(data.message);
      } catch (err) {
        console.error("Fetch error:", err);
        setConfirmBackendMessage("Unable to connect");
      }
    };

    fetchMessage();
  }, []);

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!email || !password) {
      setError("Please enter both email and password.");
      return;
    }
    const createUser = async () => {
      try {
        const response = await fetch("/api/User/Create", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ username: email, password }),
        });
        if (!response.ok)
          throw new Error("Sorry! Unable to create user. Please try again.");
        const data = await response.json();
        console.log(data);
      } catch (err) {
        console.error("Fetch error:", err);
        setError(err.message || "An unexpected error occurred.");
      }
    };

    setError("");
    createUser();
  };

  const toggleCreateAccount = () => {
    setIsCreateForm(!isCreateForm);
  };

  return (
    <section className="column center full-view-height">
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

      <footer className="footer">Backend is: {confirmBackendMessage}</footer>
    </section>
  );
};

export default Login;
