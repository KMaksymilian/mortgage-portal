import { createContext, useContext, useEffect, useState } from 'react';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(() => {
    try {
      const saved = localStorage.getItem('user');
      return saved ? JSON.parse(saved) : null;
    } catch {
      localStorage.removeItem('user');
      return null;
    }
  });

  const refreshUserData = async () => {
    if (!user?.token) return;

    try {
      const response = await fetch('/api/User/Me', {
        headers: { Authorization: `Bearer ${user.token}` },
      });

      if (!response.ok) return;

      const data = await response.json();
      setUser((prev) => {
        if (!prev) return prev;
        const updated = { ...prev, hasBirthDate: data.hasBirthDate };
        localStorage.setItem('user', JSON.stringify(updated));
        return updated;
      });
    } catch (err) {
      console.error('Błąd odświeżania profilu', err);
    }
  };

  const login = (userData) => {
    setUser(userData);
    localStorage.setItem('user', JSON.stringify(userData));
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem('user');
  };

  useEffect(() => {
    if (user?.token) refreshUserData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [user?.token]);

  return (
    <AuthContext.Provider value={{ user, login, logout, refreshUserData }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
