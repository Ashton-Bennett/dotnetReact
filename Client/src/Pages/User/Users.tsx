import { useEffect, useState } from "react";
import UserService from "../../api/userService";
import isSuccessResponse from "../../utilities/isSuccessResponse";
import type { components } from "../../types/api";
type User = components["schemas"]["User"];
import { Link } from "react-router-dom";

const Users = () => {
  const [users, setUsers] = useState<User[] | null>(null);
  const [loadingMessage, setLoadingMessage] = useState("loading");
  const [error, setError] = useState("");

  const getUsers = async () => {
    try {
      const response = await UserService.getAll();
      if (isSuccessResponse(response)) {
        setUsers(response.data.data);
      } else {
        setLoadingMessage("Error loading the users.");
      }
    } catch {
      setLoadingMessage("Error loading the users.");
    }
  };

  useEffect(() => {
    getUsers();
  }, []);

  const deleteUser = async (id: number | undefined) => {
    try {
      const response = await UserService.delete(String(id));
      if (isSuccessResponse(response)) {
        getUsers();
      } else {
        setLoadingMessage("Error loading the users.");
      }
    } catch (err) {
      setError("Unable to delete user.");
    }
  };

  return (
    <div className="column f-gap-8 center">
      {error && <p className="p-4 color-danger-1">{error}</p>}
      <h1 className="md-font">Users</h1>
      <section>
        {users ? (
          <div className="column f-gap-4">
            {users.map((user: User) => (
              <div key={user.id} className="row f-gap-4">
                <Link
                  to={`/User/Edit/${user.id}`}
                  className="no-decoration color-shade-13"
                >
                  {user.email}
                </Link>
                <button onClick={() => deleteUser(user.id)}>Delete User</button>
              </div>
            ))}
          </div>
        ) : (
          <p>{loadingMessage}</p>
        )}
      </section>
    </div>
  );
};

export default Users;
