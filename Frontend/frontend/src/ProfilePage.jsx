import { useState, useEffect } from 'react';
import { useAuth } from './AuthContext';

function ProfilePage() {
  const { user } = useAuth();
  const [profileData, setProfileData] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!user) return; 

    const token = user.token; 

    fetch('http://localhost:5254/api/profile/me', {
      headers: {
        'Authorization': `Bearer ${token}` 
      }
    })
    .then(res => {
      if (!res.ok) throw new Error("Nie udało się pobrać danych lub token jest nieważny");
      return res.json();
    })
    .then(data => setProfileData(data))
    .catch(err => setError(err.message));

  }, [user]);

  if (!user) {
    return <h2>Musisz być zalogowany, aby zobaczyć profil.</h2>;
  }

  return (
    <div>
      <h2>Profil Użytkownika</h2>
      <p>Witaj, {user.email}!</p>
      
      <h3>Twoje dane z bazy:</h3>
      {error && <p style={{color: 'red'}}>{error}</p>}
      {profileData ? (
        <pre>{JSON.stringify(profileData, null, 2)}</pre>
      ) : (
        <p>Ładowanie danych...</p>
      )}
    </div>
  );
}

export default ProfilePage;