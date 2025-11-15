import { GoogleLogin } from '@react-oauth/google';
import { useAuth } from './AuthContext';
import { useNavigate } from 'react-router-dom';

function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleGoogleSuccess = (credentialResponse) => {
    // 1. Mamy token od Google (credentialResponse.credential)
    console.log("Token z Google:", credentialResponse);

    // 2. Wysyłamy ten token do naszego backendu
    fetch('http://localhost:5254/api/auth/google-login', { // WAŻNY ADRES!
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ token: credentialResponse.credential })
    })
    .then(res => res.json())
    .then(backendResponse => {
      // 4. Backend zweryfikował token i odesłał nam dane usera + SWÓJ token
      //    (np. { email: "...", token: "nasz-jwt-token" })
      login(backendResponse); 
      // 5. Przekieruj na stronę profilu
      navigate('/profile'); 
    })
    .catch(err => console.error("Błąd logowania na backendzie:", err));
  };

  return (
    <div>
      <h2>Logowanie</h2>
      <p>Zaloguj się przez Google:</p>
      <GoogleLogin
        onSuccess={handleGoogleSuccess}
        onError={() => {
          console.log('Login Failed');
        }}
      />
    </div>
  );
}

export default LoginPage;