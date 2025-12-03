import type { components } from "./types/api";
import { createContext, useState, type ReactNode } from "react";
type User = components["schemas"]["User"];
import tokenStore from "./utilities/tokenStore";

interface AuthContextType {
  isAuthenticated: boolean;
  storeLogin: (token: string, user: User) => void;
  storeLogout: () => void;
  accessToken: string | null;
  currentUser: User | null;
  setCurrentUser: React.Dispatch<User | null>;
}

// Create context
const AuthContext = createContext<AuthContextType | undefined>(undefined);

const AuthProviderComponent = ({ children }: { children: ReactNode }) => {
  const [currentUser, setCurrentUser] = useState<User | null>(null);

  const storeLogin = (token: string, userInfo: User) => {
    tokenStore.set(token);
    setCurrentUser(userInfo);
  };

  const storeLogout = () => {
    tokenStore.set(null);
    setCurrentUser(null);
  };

  const isAuthenticated = !!tokenStore.get();
  const accessToken = tokenStore.get();

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated,
        storeLogin,
        storeLogout,
        accessToken,
        currentUser,
        setCurrentUser,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export { AuthProviderComponent as AuthProvider, AuthContext };
