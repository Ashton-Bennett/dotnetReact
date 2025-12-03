import useAuth from "./AuthContext";

const useRoleChecker = () => {
  const { currentUser } = useAuth();

  const hasRole = (role: string): boolean => {
    return currentUser?.roles?.includes(role) ?? false;
  };

  const isManager = (): boolean => hasRole("Manager");
  const isAdmin = (): boolean => hasRole("Admin");
  const isUser = (): boolean => hasRole("User");

  return { hasRole, isManager, isAdmin, isUser };
};

export default useRoleChecker;
