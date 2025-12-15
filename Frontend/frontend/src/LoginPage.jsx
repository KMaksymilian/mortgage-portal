import { GoogleLogin } from '@react-oauth/google';
import { useAuth } from './AuthContext';
import { useNavigate } from 'react-router-dom';

function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleGoogleSuccess = (credentialResponse) => {
    fetch('http://localhost:5254/api/auth/google-login', { 
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ token: credentialResponse.credential })
    })
    .then(res => res.json())
    .then(backendResponse => {
      
      const isComplete = backendResponse.hasBirthDate;

      const userData = {
          ...backendResponse,
          hasBirthDate: isComplete // Przepisujemy to do stanu
      };

      login(userData); 
      
      if (isComplete) {
          navigate('/search'); // Jeśli ma datę -> idź do szukania
      } else {
          navigate('/complete-profile'); // Jeśli nie ma daty -> idź uzupełnić
      }
    })
    .catch(err => console.error("Błąd logowania:", err));
  };

  return (
    <div className="card">
      <h2>Logowanie</h2>
      <GoogleLogin onSuccess={handleGoogleSuccess} onError={() => console.log('Login Failed')} />
    </div>
  );
}

export default LoginPage;