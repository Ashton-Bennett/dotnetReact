import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import isSuccessResponse from "../utilities/isSuccessResponse";
import UserService from "../api/userService";
import RoleCheckBoxes from "../components/RoleCheckboxes";
import type { components } from "../types/api";
import useAuth from "../hooks/AuthContext";
type User = components["schemas"]["User"];

interface UpdatePasswordCardProps {
  setFormToShow: React.Dispatch<
    React.SetStateAction<"editUser" | "updatePassword">
  >;
}

const EditUserForm = ({ setFormToShow }: UpdatePasswordCardProps) => {
  const { id } = useParams();
  const { currentUser, setCurrentUser } = useAuth();
  const navigate = useNavigate();
  const [user, setUser] = useState<User | null>(null);
  const [error, setError] = useState<string>("");

  useEffect(() => {
    const getUser = async () => {
      try {
        if (!id) throw new Error("Missing ID");
        const reponse = await UserService.getById(id);
        if (isSuccessResponse(reponse)) {
          setUser(reponse.data.data);
        }
      } catch {
        setError("Failed to load user.");
      }
    };
    getUser();
  }, []);

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (!id) throw new Error("Missing ID");
      if (!user) throw new Error("User not found");

      const response = await UserService.update(id, user);
      if (response) {
        navigate(-1);
      }

      if (currentUser && currentUser.id == Number(id)) {
        setCurrentUser(user);
      }
    } catch (err) {
      setError(
        `Error: ${
          // @ts-ignore
          err.response?.data?.message || "An unexpected error occurred."
        }`
      );
    }
  };

  return (
    <form
      onSubmit={handleSave}
      className="white-border radius p-8 column f-gap-8"
    >
      <h1 className="text-center md-font">Edit User</h1>

      {error && <p className="color-danger-2">{error}</p>}

      <div className="row f-gap-4">
        <label>Name</label>
        <input
          type="text"
          value={user?.username ?? ""}
          onChange={(e) => setUser({ ...user!, username: e.target.value })}
          required
        />
      </div>

      {/* Email */}
      <div className="row f-gap-4">
        <label>Email</label>
        <input
          type="email"
          value={user?.email ?? ""}
          onChange={(e) => setUser({ ...user!, email: e.target.value })}
          required
        />
      </div>

      {/* DOB */}
      <div className="row f-gap-4">
        <label>Date Of Birth</label>
        <input
          type="date"
          value={user?.dateOfBirth ?? ""}
          onChange={(e) => setUser({ ...user!, dateOfBirth: e.target.value })}
          required
        />
      </div>

      <RoleCheckBoxes userRoles={user?.roles} setUser={setUser} />

      <div className="row space-between f-gap-4">
        <button type="submit">Save Changes</button>
        <button type="button" onClick={() => setFormToShow("updatePassword")}>
          Update Password
        </button>
        <button type="button" onClick={() => navigate(-1)}>
          Cancel
        </button>
      </div>
    </form>
  );
};

export default EditUserForm;
