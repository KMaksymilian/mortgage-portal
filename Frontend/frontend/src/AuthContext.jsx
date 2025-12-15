import { createContext, useContext, useState, useEffect } from 'react';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  // Przechowujemy user w formacie: { token: "...", email: "...", hasBirthDate: false/true }
  const [user, setUser] = useState(() => {
    const saved = localStorage.getItem('user');
    return saved ? JSON.parse(saved) : null;
  });

  // Funkcja do pobierania aktualnych danych profilu (czy ma datę urodzenia?)

  const refreshUserData = async () => {
    if (!user?.token) return;

    try {
      const response = await fetch('http://localhost:5254/api/User/Me', { 
        headers: {
          'Authorization': `Bearer ${user.token}`
        }
      });

      if (response.ok) {
        const data = await response.json();
        setUser(prev => {
          // Upewniamy się, że zapisujemy to w stanie
          const updated = { ...prev, hasBirthDate: data.hasBirthDate };
          localStorage.setItem('user', JSON.stringify(updated));
          return updated;
        });
      }
    } catch (err) {
      console.error("Błąd odświeżania profilu", err);
    }
  };

  // Login (uproszczony)
  const login = (userData) => {
    setUser(userData);
    localStorage.setItem('user', JSON.stringify(userData));
    // Po zalogowaniu od razu sprawdzamy profil
    // Lepiej wywołać to w komponencie po przekierowaniu lub w useEffect
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem('user');
  };

  useEffect(() => {
      if(user?.token) {
          refreshUserData();
      }
  }, []);

  return (
    <AuthContext.Provider value={{ user, login, logout, refreshUserData }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);