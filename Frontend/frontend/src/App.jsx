import { Routes, Route, NavLink, Navigate } from 'react-router-dom';
import { useAuth } from './AuthContext';

import LoginPage from './LoginPage.jsx';
import ProfilePage from './ProfilePage.jsx';
import HomePage from './HomePage.jsx';
import OfferSearchPage from './OfferSearchPage.jsx';
import CompleteProfilePage from './CompleteProfilePage.jsx';
import PastOffersPage from './PastOffersPage.jsx';
import FinalizeApplicationPage from './FinalizeApplicationPage.jsx';

import { useAdminAuth } from './admin/AdminAuthContext';
import AdminLoginPage from './admin/AdminLoginPage';
import AdminOffersPage from './admin/AdminOffersPage';
import RequireAdmin from './admin/RequireAdmin';

import './App.css';

const linkClass = ({ isActive }) => `navlink${isActive ? ' active' : ''}`;

function App() {
  const { user, logout } = useAuth();
  const { admin } = useAdminAuth();

  const isAdmin = !!admin?.token;

  return (
    <div className="app">
      <header className="topbar">
        <div className="container topbar-inner">
          <div className="brand">
            <span className="brand-dot" />
            Mortgage Portal
          </div>

          <nav className="navlinks">
            {/* ADMIN: ma dostęp tylko do panelu admina */}
            {isAdmin ? (
              <>
                <NavLink to="/admin/offers" className={linkClass}>Panel admina</NavLink>
                {/* Wylogowanie admina jest w AdminOffersPage (przycisk), więc tu nic nie musisz dawać */}
              </>
            ) : user ? (
              <>
                <NavLink to="/search" className={linkClass}>Kalkulator</NavLink>
                <NavLink to="/history" className={linkClass}>Historia</NavLink>
                <NavLink to="/profile" className={linkClass}>Profil</NavLink>
                <button className="btn" onClick={logout}>
                  Wyloguj ({user.email})
                </button>
              </>
            ) : (
              <>
                <NavLink to="/" end className={linkClass}>Home</NavLink>
                <NavLink to="/login" className={linkClass}>Zaloguj</NavLink>
                <NavLink to="/admin/login" className={linkClass}>Admin</NavLink>
              </>
            )}
          </nav>
        </div>
      </header>

      <main className="container" style={{ padding: '26px 0 40px' }}>
        <Routes>
          {/* ADMIN: wszystko przekierowujemy do panelu admina */}
          {isAdmin ? (
            <>
              <Route path="/admin/login" element={<Navigate to="/admin/offers" replace />} />
              <Route
                path="/admin/offers"
                element={
                  <RequireAdmin>
                    <AdminOffersPage />
                  </RequireAdmin>
                }
              />
              <Route path="*" element={<Navigate to="/admin/offers" replace />} />
            </>
          ) : (
            <>
              {/* NORMALNY USER / GOŚĆ */}
              <Route
                path="/"
                element={user ? <Navigate to="/search" replace /> : <HomePage />}
              />

              {/* Admin login dostępny tylko jeśli NIE jest zalogowany admin */}
              <Route path="/admin/login" element={<AdminLoginPage />} />

              <Route path="/login" element={<LoginPage />} />
              <Route path="/profile" element={<ProfilePage />} />
              <Route path="/search" element={<OfferSearchPage />} />
              <Route path="/complete-profile" element={<CompleteProfilePage />} />
              <Route path="/history" element={<PastOffersPage />} />
              <Route path="/finalize-application" element={<FinalizeApplicationPage />} />

              {/* Jeśli ktoś wejdzie w /admin/offers bez admina -> na admin login */}
              <Route path="/admin/offers" element={<Navigate to="/admin/login" replace />} />
            </>
          )}
        </Routes>
      </main>
    </div>
  );
}

export default App;
