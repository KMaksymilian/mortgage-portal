import { Routes, Route, Link } from 'react-router-dom';
import { useAuth } from './AuthContext';
import LoginPage from './LoginPage.jsx';
import ProfilePage from './ProfilePage.jsx';
import HomePage from './HomePage.jsx';
import OfferSearchPage from './OfferSearchPage.jsx'; // <--- 1. IMPORT
import CompleteProfilePage from './BirthDateSite.jsx';

function App() {
  const { user, logout } = useAuth();

  return (
    <div>
      <nav>
        <Link to="/">Home</Link> | 
        
        {/* Menu dla zalogowanych */}
        {user ? (
          <>
            <Link to="/search">Wyszukaj Ofertę</Link> | {/* <--- 2. LINK */}
            <Link to="/profile">Profil</Link> | 
            <button onClick={logout} style={{ marginLeft: '10px' }}>Wyloguj ({user.email})</button>
          </>
        ) : (
          <Link to="/login">Zaloguj</Link>
        )}
      </nav>

      <hr />

      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/profile" element={<ProfilePage />} />
        <Route path="/search" element={<OfferSearchPage />} />
        <Route path="/complete-profile" element={<CompleteProfilePage />} />
      </Routes>
    </div>
  );
}

export default App;