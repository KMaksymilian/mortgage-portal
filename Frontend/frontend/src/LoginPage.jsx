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
    const isComplete = !!backendResponse.hasBirthDate;

    const userData = {
      ...backendResponse,
      hasBirthDate: isComplete,
    };

    login(userData);
    navigate(isComplete ? '/search' : '/complete-profile');
  } catch (err) {
    console.error('Błąd logowania:', err);
  }
};

  return (
    <div className="card">
      <h2>Logowanie</h2>
      <GoogleLogin onSuccess={handleGoogleSuccess} onError={() => console.log('Login Failed')} />
    </div>
  );
}

export default LoginPage;