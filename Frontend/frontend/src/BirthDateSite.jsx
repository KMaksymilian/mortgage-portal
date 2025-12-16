import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from './AuthContext';

function CompleteProfilePage() {
  const [birthDate, setBirthDate] = useState('');
  const [error, setError] = useState(null);
  const { user, refreshUserData } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);

    try {
      const response = await fetch('http://localhost:5254/api/User/BirthDate', {
        method: 'POST', // Zgodnie z Twoim kontrolerem
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${user.token}`
        },
        body: JSON.stringify({ birthDate: birthDate })
      });

      if (!response.ok) {
        throw new Error('Nie udało się zapisać daty.');
      }

      // SUKCES!
      // 1. Odświeżamy kontekst, żeby aplikacja wiedziała, że user ma już datę
      await refreshUserData();
      
      // 2. Przekierowujemy z powrotem do szukania ofert
      navigate('/search'); 

    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className="card">
      <h2>Dokończ konfigurację konta</h2>
      <p>Abyśmy mogli przygotować ofertę, potrzebujemy Twojej daty urodzenia.</p>
      
      <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '15px', maxWidth: '300px', margin: '20px auto' }}>
        <label>
          Data urodzenia:
          <input 
            type="date" 
            value={birthDate} 
            onChange={(e) => setBirthDate(e.target.value)} 
            required 
            style={{ width: '100%', padding: '8px', marginTop: '5px' }}
          />
        </label>

        <button type="submit">Zapisz i przejdź dalej</button>
      </form>
      
      {error && <p style={{ color: 'red' }}>{error}</p>}
    </div>
  );
}

export default CompleteProfilePage;