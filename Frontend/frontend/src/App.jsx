import { Routes, Route, NavLink } from 'react-router-dom';
import { useAuth } from './AuthContext';
import LoginPage from './LoginPage.jsx';
import ProfilePage from './ProfilePage.jsx';
import HomePage from './HomePage.jsx';
import OfferSearchPage from './OfferSearchPage.jsx';
import CompleteProfilePage from './BirthDateSite.jsx';
import PastOffersPage from './PastOffersPage.jsx';
import './App.css';

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
            <NavLink to="/" end className={linkClass}>Home</NavLink>

            {user ? (
              <>
                <NavLink to="/search" className={linkClass}>Kalkulator</NavLink>
                <NavLink to="/history" className={linkClass}>Historia</NavLink>
                <NavLink to="/profile" className={linkClass}>Profil</NavLink>
                <button className="btn" onClick={logout}>
                  Wyloguj ({user.email})
                </button>
              </>
            ) : (
              <NavLink to="/login" className={linkClass}>Zaloguj</NavLink>
            )}
          </nav>
        </div>
      </header>

      <main className="container" style={{ padding: '26px 0 40px' }}>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/profile" element={<ProfilePage />} />
          <Route path="/search" element={<OfferSearchPage />} />
          <Route path="/complete-profile" element={<CompleteProfilePage />} />
          <Route path="/history" element={<PastOffersPage />} />
        </Routes>
      </main>
    </div>
  );
}

export default App;
