import { useState, useEffect } from 'react';
import { useAuth } from './AuthContext';

function ProfilePage() {
  const { user } = useAuth();
  const [profileData, setProfileData] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Jeśli nie ma usera, nie pobieramy
    if (!user) return; 

    const fetchProfileData = async () => {
      try {
        const response = await fetch('/api/Dictionary/DocumentAndJobTypes', {
          method: 'GET',
          headers: {
            'Authorization': `Bearer ${user.token}`,
            'Content-Type': 'application/json'
          }
        });

        if (!response.ok) {
          throw new Error("Nie udało się pobrać danych profilowych.");
        }

        const data = await response.json();
        setProfileData(data);

      } catch (err) {
        console.error(err);
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchProfileData();

  }, [user]);

  if (!user) {
    return <h2>Musisz być zalogowany, aby zobaczyć profil.</h2>;
  }

  return (
    <div className="card">
      <h2>Twój Profil Użytkownika</h2>
      <p style={{ color: '#888' }}>Zalogowany jako: <strong>{user.email}</strong></p>
      
      <hr style={{ margin: '20px 0', borderColor: '#444' }} />

      {loading && <p>Pobieranie danych z urzędu (losowanie zawodu)...</p>}
      
      {error && <div style={{ color: 'red', border: '1px solid red', padding: '10px' }}>{error}</div>}

      {!loading && profileData && (
        <div style={{ textAlign: 'left', maxWidth: '500px', margin: '0 auto' }}>

          {/* NOWA SEKCJA: Dane osobowe */}
          <div style={{ marginBottom: '20px', padding: '15px', border: '1px solid #ccc', borderRadius: '8px' }}>
              <h3>Dane Osobowe</h3>
              <p><strong>Imię i Nazwisko:</strong> {profileData.firstName} {profileData.lastName}</p>
              <p><strong>Email:</strong> {profileData.email}</p>
              <p><strong>Data urodzenia:</strong> {profileData.birthDate ? new Date(profileData.birthDate).toLocaleDateString() : 'Brak'}</p>
          </div>
          
          {/* Sekcja Zawodu */}
          <div style={{ marginBottom: '20px', padding: '15px', border: '1px solid #646cff', borderRadius: '8px' }}>
            <h3 style={{ marginTop: 0 }}>Twoja Praca</h3>
            {profileData.job ? (
              <>
                <p><strong>Stanowisko:</strong> {profileData.job.name}</p>
                <p><strong>Opis:</strong> {profileData.job.description}</p>
              </>
            ) : (
              <p>Brak przypisanej pracy.</p>
            )}
          </div>

          {/* Sekcja Dokumentu */}
          <div style={{ padding: '15px', border: '1px solid #4CAF50', borderRadius: '8px' }}>
            <h3 style={{ marginTop: 0 }}>Twój Dokument Tożsamości</h3>
            {profileData.document ? (
              <>
                <p><strong>Typ dokumentu:</strong> {profileData.document.name}</p>
                <p><strong>Opis:</strong> {profileData.document.description}</p>
              </>
            ) : (
              <p>Brak przypisanego dokumentu.</p>
            )}
          </div>

        </div>
      )}
    </div>
  );
}

export default ProfilePage;