import React, { type ReactNode } from "react";
import Navbar from "../components/NavBar";

interface NavBarIncludedLayoutProps {
  children: ReactNode;
  userLoggedIn: boolean;
}

const NavBarIncludedLayout: React.FC<NavBarIncludedLayoutProps> = ({
  children,
}) => {
  return (
    <>
      <Navbar />
      <main className="p-8 flex-1">{children}</main>
    </>
  );
};

export default NavBarIncludedLayout;
