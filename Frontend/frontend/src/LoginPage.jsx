import { GoogleLogin } from '@react-oauth/google';
import { useAuth } from './AuthContext';
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

      const backendResponse = await res.json();

      // Tworzymy obiekt użytkownika
      const userData = {
        token: backendResponse.token,
        email: backendResponse.email,
        firstName: backendResponse.firstName,
        earnings: backendResponse.earnings,
        birthDate: backendResponse.birthDate,
        jobStartDate: backendResponse.jobStartDate,
        jobEndDate: backendResponse.jobEndDate
      };

      login(userData);

      // === LOGIKA PRZEKIEROWANIA ===
      const pendingQuoteId = localStorage.getItem('selectedQuoteId');

      if (pendingQuoteId) {
        // Scenariusz A: Mamy wybraną ofertę -> idziemy do finalizacji (tam też jest formularz uzupełniania)
        console.log("Wykryto wybraną ofertę, przekierowanie do finalizacji...");
        navigate('/finalize-application');
      } else {
        // Scenariusz B: Zwykłe logowanie -> sprawdzamy kompletność profilu
        
        // WARUNEK KOMPLETNOŚCI: Musi mieć datę urodzenia, zarobki ORAZ datę startu pracy
        const hasBirthDate = !!backendResponse.birthDate;
        const hasEarnings = backendResponse.earnings !== null && backendResponse.earnings !== undefined; // Bo zarobki mogą wynosić 0
        const hasJobStart = !!backendResponse.jobStartDate;

        const isProfileComplete = hasBirthDate && hasEarnings && hasJobStart;
        
        if (isProfileComplete) {
            navigate('/search'); // Profil gotowy -> do kalkulatora
        } else {
            navigate('/complete-profile'); // Brakuje danych -> uzupełnij profil
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