import { useState, useEffect } from "react";
import "./App.css";
import "./styles/site.css";

function App() {
  const [confirmBackendMessage, setconfirmBackendMessage] =
    useState<string>("Not Connected.");
  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [error, setError] = useState<string>("");

  useEffect(() => {
    const fetchMessage = async () => {
      try {
        const res = await fetch("/api/greeting");
        if (!res.ok) throw new Error("Network response was not ok");
        const data = await res.json();
        setconfirmBackendMessage(data.message);
      } catch (err) {
        console.error("Fetch error:", err);
        setconfirmBackendMessage("Unable to connect");
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

    setError("");

    console.log("Logging in with", { email, password });
    alert(`Logged in with email: ${email}`);
  };

  return (
    <>
      <div>
        <form
          onSubmit={handleSubmit}
          className="column f-gap-4 white-border p-4 radius"
        >
          <h1 className="lg-font">Login</h1>

          {error && <p>{error}</p>}

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
          <button type="submit">Login</button>
        </form>
      </div>

      <footer className="footer">Backend is: {confirmBackendMessage}</footer>
    </>
  );
}

export default App;
