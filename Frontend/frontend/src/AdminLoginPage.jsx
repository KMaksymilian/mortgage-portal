import { GoogleLogin } from '@react-oauth/google';
import { useNavigate } from 'react-router-dom';
import { useAuth } from './AuthContext';
import { request } from './api/http';

export default function AdminLoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSuccess = async (cred) => {
    try {
      const data = await request('/api/auth/admin/google-login', {
        method: 'POST',
        body: { token: cred.credential },
      });

      const role = data.role ?? data.Role ?? 'User';
      login(data, role); // musi zawierać token + role
      navigate('/admin');
    } catch (err) {
      alert(`Brak dostępu admina: ${err.message}`);
    }
  };

  return (
    <div className="card" style={{ maxWidth: 520, margin: '28px auto' }}>
      <h2>Panel Admina</h2>
      <p style={{ color: 'var(--muted)' }}>
        Zaloguj się kontem z rolą Admin.
      </p>
      <GoogleLogin onSuccess={handleSuccess} onError={() => alert('Błąd logowania Google')} />
    </div>
  );
}
