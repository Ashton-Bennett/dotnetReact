import { useState, useEffect } from "react";
import UserService from "../api/userService";
import type { Dispatch, SetStateAction } from "react";
import type { components } from "../types/api";
type User = components["schemas"]["User"];

export interface RoleDto {
  id: number;
  name: string;
}

interface RoleCheckBoxesProps {
  userRoles: string[] | null | undefined;
  setUser: Dispatch<SetStateAction<User | null>>;
}

const RoleCheckBoxes = ({ userRoles, setUser }: RoleCheckBoxesProps) => {
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [readRoleOnly, setReadRoleOnly] = useState<boolean>(false);

  useEffect(() => {
    const getRoles = async () => {
      try {
        const reponse = await UserService.getRoles();
        if (reponse) {
          setRoles(reponse.data);
        }
      } catch {
        console.error("Unable to get roles");
        setReadRoleOnly(true);
      }
    };
    getRoles();
  }, []);

  const handleRoleChange = (roleName: string, checked: boolean) => {
    setUser((prev) => {
      if (!prev) return prev;

      const currentRoles = prev?.roles ?? [];

      const newRoles = checked
        ? [...currentRoles.flat(), roleName] // add role
        : currentRoles.filter((r) => r !== roleName); // remove role

      return { ...prev, roles: newRoles };
    });
  };

  return (
    <div className={`${readRoleOnly ? "row" : "column"} f-gap-4`}>
      <p>User Roles</p>

      <div className="column f-gap-4">
        {roles.map((role) => (
          <label key={role.id} className="row f-gap-2 pointer">
            <input
              type="checkbox"
              value={role.id}
              checked={userRoles?.includes(role.name) ?? false}
              onChange={(e) => handleRoleChange(role.name, e.target.checked)}
            />
            {role.name}
          </label>
        ))}
      </div>

      {readRoleOnly && <input readOnly value={userRoles?.join(" ")} />}
    </div>
  );
};

export default RoleCheckBoxes;
