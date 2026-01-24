import { GoogleLogin } from '@react-oauth/google';
import { useAuth } from './AuthContext'; // Upewnij się, że ścieżka jest poprawna
import { useNavigate } from 'react-router-dom';

function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleGoogleSuccess = async (credentialResponse) => {
    try {
      const res = await fetch('/api/auth/google-login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ token: credentialResponse.credential }),
      });

      if (!res.ok) {
        const msg = await res.text();
        throw new Error(msg || 'Błąd logowania');
      }

      // 1. Pobieramy pełne dane z backendu (UserDto)
      // Oczekujemy: { token, email, earnings, birthDate, jobStartDate, jobEndDate }
      const backendResponse = await res.json();

      // 2. Tworzymy obiekt użytkownika dla AuthContext
      // Ważne: Przepisujemy pola finansowe, żeby FinalizeApplicationPage je widział
      const userData = {
        token: backendResponse.token,
        email: backendResponse.email,
        firstName: backendResponse.firstName, // Jeśli backend to zwraca
        // Dane do wniosku:
        earnings: backendResponse.earnings,
        birthDate: backendResponse.birthDate,
        jobStartDate: backendResponse.jobStartDate,
        jobEndDate: backendResponse.jobEndDate
      };

      // 3. Logujemy w kontekście aplikacji
      login(userData);

      // 4. Logika przekierowania
      const pendingQuoteId = localStorage.getItem('selectedQuoteId');

      if (pendingQuoteId) {
        // SCENARIUSZ A: Użytkownik wybrał ofertę przed logowaniem -> Idziemy ją sfinalizować
        console.log("Wykryto wybraną ofertę, przekierowanie do finalizacji...");
        navigate('/finalize-application');
      } else {
        // SCENARIUSZ B: Zwykłe logowanie
        // Sprawdzamy czy profil jest kompletny (np. czy ma datę urodzenia)
        const isProfileComplete = !!backendResponse.birthDate; 
        
        if (isProfileComplete) {
            navigate('/search'); // Ma dane -> może szukać
        } else {
            navigate('/complete-profile'); // Nie ma danych -> niech uzupełni profil
        }
      }

    } catch (err) {
      console.error('Błąd logowania:', err);
      alert("Logowanie nie powiodło się. Spróbuj ponownie.");
    }
  };

  return (
    <div className="card" style={{maxWidth: '400px', margin: '50px auto', textAlign: 'center', padding: '40px'}}>
      <h2 style={{color: 'var(--brand)', marginBottom: '30px'}}>Logowanie</h2>
      <div style={{display: 'flex', justifyContent: 'center'}}>
        <GoogleLogin 
            onSuccess={handleGoogleSuccess} 
            onError={() => console.log('Login Failed')} 
            theme="filled_black"
            shape="pill"
        />
      </div>
    </div>
  );
}

export default LoginPage;