import { Routes, Route, Link } from 'react-router-dom';
import { useAuth } from './AuthContext';
import LoginPage from './LoginPage.jsx';
import ProfilePage from './ProfilePage.jsx';
import HomePage from './HomePage.jsx'; // Stwórz prosty plik HomePage.jsx

function App() {
  const { user, logout } = useAuth();

  return (
    <div>
      {/* Prosta nawigacja (zakładki) */}
      <nav>
        <Link to="/">Home</Link> | 
        <Link to="/profile">Profil</Link> | 
        {user ? (
          <button onClick={logout}>Wyloguj ({user.email})</button>
        ) : (
          <Link to="/login">Zaloguj</Link>
        )}
      </nav>

      <hr />

      {/* Kontener na aktualnie wybraną stronę */}
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginPage />} />
        {/* Możemy dodać "Protected Route", ale na razie prosto: */}
        <Route path="/profile" element={<ProfilePage />} />
      </Routes>
    </div>
  );
}

export default App;
