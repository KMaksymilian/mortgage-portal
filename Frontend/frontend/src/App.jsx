import { Routes, Route, Link } from 'react-router-dom';
import { useAuth } from './AuthContext';
import LoginPage from './LoginPage.jsx';
import ProfilePage from './ProfilePage.jsx';
import HomePage from './HomePage.jsx';
import OfferSearchPage from './OfferSearchPage.jsx'; // <--- 1. IMPORT
import CompleteProfilePage from './BirthDateSite.jsx';
import PastOffersPage from './PastOffersPage.jsx';

function App() {
  const { user, logout } = useAuth();

  return (
    <div>
      <nav>
        <Link to="/">Home</Link> | 
        
        {user ? (
          <>
            <Link to="/search">Kalkulator</Link> | 
            {/* 2. DODAJ LINK W MENU */}
            <Link to="/history">Historia Ofert</Link> | 
            
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
        
        {}
        <Route path="/history" element={<PastOffersPage />} />
      </Routes>
    </div>
  );
}

export default App;