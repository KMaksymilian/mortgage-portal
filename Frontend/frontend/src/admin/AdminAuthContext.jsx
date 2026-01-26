import { createContext, useContext, useState } from "react";

const AdminAuthContext = createContext(null);

export function AdminAuthProvider({ children }) {
  const [admin, setAdmin] = useState(() => {
    const saved = localStorage.getItem("admin");
    return saved ? JSON.parse(saved) : null;
  });

  const loginAdmin = (data) => {
    setAdmin(data);
    localStorage.setItem("admin", JSON.stringify(data));
  };

  const logoutAdmin = () => {
    setAdmin(null);
    localStorage.removeItem("admin");
  };

  return (
    <AdminAuthContext.Provider value={{ admin, loginAdmin, logoutAdmin }}>
      {children}
    </AdminAuthContext.Provider>
  );
}

export const useAdminAuth = () => useContext(AdminAuthContext);
