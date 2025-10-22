import "./styles/site.css";
import { Routes, Route } from "react-router-dom";
import Home from "./Pages/Home.js";
import Login from "./Pages/Login.js";

const App = () => {
  return (
    <>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/login" element={<Login />} />

        {/* 404 fallback */}
        <Route path="*" element={<h2>Page not found</h2>} />
      </Routes>
    </>
  );
};

export default App;
