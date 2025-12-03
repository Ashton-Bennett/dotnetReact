import { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import useAuth from "../hooks/AuthContext";
import isSuccessResponse from "../utilities/isSuccessResponse";
import UserService from "../api/userService";
import useLogout from "../hooks/useLogout";

interface UpdatePasswordCardProps {
  setFormToShow: React.Dispatch<
    React.SetStateAction<"editUser" | "updatePassword">
  >;
}

const UpdatePasswordCard = ({ setFormToShow }: UpdatePasswordCardProps) => {
  const { id } = useParams();
  const logout = useLogout();
  const navigate = useNavigate();
  const { currentUser } = useAuth();
  const [currentPassword, setCurrentPassword] = useState<string>("");
  const [newPassword, setNewPassword] = useState<string>("");
  const [newPasswordVerification, setNewPasswordVerification] =
    useState<string>("");
  const [validationMessage, setValidationMessage] = useState<string>("");
  const [colorResponseTextGreen, setColorResponseTextGreen] =
    useState<boolean>(false);

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (!id) throw new Error("Missing ID");

      if (!passwordsMatch(newPassword, newPasswordVerification)) {
        return;
      }

      const response = await UserService.updatePassword(
        id,
        currentPassword,
        newPassword
      );
      const isWorking = isSuccessResponse(response);
      if (isWorking) {
        if (currentUser && currentUser.id === Number(id)) {
          setColorResponseTextGreen(true);
          setValidationMessage(response.data.data);
          setTimeout(() => {
            logout();
          }, 5000);
        } else {
          navigate(-1);
        }
      } else {
        setColorResponseTextGreen(false);
        setValidationMessage("Sorry that didnt work.");
      }
    } catch (err) {
      setColorResponseTextGreen(false);
      setValidationMessage(
        `Error: ${
          // @ts-ignore
          err.response?.data?.message || "An unexpected error occurred."
        }`
      );
    }
  };

  const passwordsMatch = (firstEntry: string, secondEntry: string) => {
    if (firstEntry === secondEntry) {
      setValidationMessage("");
      return true;
    }
    setColorResponseTextGreen(false);
    setValidationMessage("New Passwords must match");
    return false;
  };

  return (
    <div className="column center f-gap-8">
      <form
        onSubmit={handleSave}
        className="white-border radius p-8 column f-gap-8"
      >
        <h1 className="text-center md-font">Update Password</h1>

        {validationMessage && (
          <p style={{ color: colorResponseTextGreen ? "green" : "red" }}>
            {validationMessage}
          </p>
        )}

        {/* Current */}
        <div className="row f-gap-4">
          <label>Current</label>
          <input
            type="password"
            onChange={(e) => setCurrentPassword(e.target.value)}
            required
          />
        </div>

        {/* New */}
        <div className="row f-gap-4">
          <label>New</label>
          <input
            type="password"
            onChange={(e) => setNewPassword(e.target.value)}
            required
          />
        </div>

        {/* Verify New */}
        <div className="row f-gap-4">
          <label>New</label>
          <input
            type="password"
            onChange={(e) => setNewPasswordVerification(e.target.value)}
            required
          />
        </div>
        <div className="row space-between f-gap-4">
          <button type="submit">Save Changes</button>
          <button type="button" onClick={() => setFormToShow("editUser")}>
            Edit User
          </button>
          <button type="button" onClick={() => navigate(-1)}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
};

export default UpdatePasswordCard;
