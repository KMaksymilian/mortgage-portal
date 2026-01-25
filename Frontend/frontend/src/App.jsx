import { Routes, Route, NavLink, Navigate } from 'react-router-dom'; // <--- ZMIANA: Dodaj Navigate
import { useAuth } from './AuthContext';
import LoginPage from './LoginPage.jsx';
import ProfilePage from './ProfilePage.jsx';
import HomePage from './HomePage.jsx';
import OfferSearchPage from './OfferSearchPage.jsx';
import CompleteProfilePage from './CompleteProfilePage.jsx';
import PastOffersPage from './PastOffersPage.jsx';
import './App.css';
import FinalizeApplicationPage from './FinalizeApplicationPage.jsx';

const linkClass = ({ isActive }) => `navlink${isActive ? ' active' : ''}`;

function App() {
  const { user, logout } = useAuth();

  return (
    <div className="app">
      <header className="topbar">
        <div className="container topbar-inner">
          <div className="brand">
            <span className="brand-dot" />
            Mortgage Portal
          </div>

          <nav className="navlinks">
            {/* 1. UKRYWAMY LINK HOME DLA ZALOGOWANYCH */}
            {user ? (
              <>
                {/* Tutaj są linki tylko dla zalogowanych */}
                <NavLink to="/search" className={linkClass}>Kalkulator</NavLink>
                <NavLink to="/history" className={linkClass}>Historia</NavLink>
                <NavLink to="/profile" className={linkClass}>Profil</NavLink>
                <button className="btn" onClick={logout}>
                  Wyloguj ({user.email})
                </button>
              </>
            ) : (
              <>
                {/* Tutaj są linki dla NIEZALOGOWANYCH */}
                <NavLink to="/" end className={linkClass}>Home</NavLink> {/* Przeniesione tutaj */}
                <NavLink to="/login" className={linkClass}>Zaloguj</NavLink>
              </>
            )}
          </nav>
        </div>
      </header>

      <main className="container" style={{ padding: '26px 0 40px' }}>
        <Routes>
          {/* 2. LOGIKA PRZEKIEROWANIA */}
          {/* Jeśli user zalogowany -> idź do /search. Jeśli nie -> pokaż HomePage */}
          <Route 
            path="/" 
            element={user ? <Navigate to="/search" replace /> : <HomePage />} 
          />
          
          <Route path="/login" element={<LoginPage />} />
          <Route path="/profile" element={<ProfilePage />} />
          <Route path="/search" element={<OfferSearchPage />} />
          <Route path="/complete-profile" element={<CompleteProfilePage />} />
          <Route path="/history" element={<PastOffersPage />} />
          <Route path="/finalize-application" element={<FinalizeApplicationPage />} />
        </Routes>
      </main>
    </div>
  );
}

export default App;