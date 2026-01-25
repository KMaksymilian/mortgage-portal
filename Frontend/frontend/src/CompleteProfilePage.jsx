import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from './AuthContext';

function CompleteProfilePage() {
    const { user, login } = useAuth(); // login użyjemy do aktualizacji kontekstu
    const navigate = useNavigate();
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState(null);

    // Inicjalizujemy tym, co już mamy (np. imię z Google), reszta pusta
    const [formData, setFormData] = useState({
        earnings: user?.earnings || '',
        birthDate: user?.birthDate ? user.birthDate.split('T')[0] : '',
        jobStartDate: user?.jobStartDate ? user.jobStartDate.split('T')[0] : '',
        jobEndDate: user?.jobEndDate ? user.jobEndDate.split('T')[0] : ''
    });

    const handleSubmit = async (e) => {
        e.preventDefault();
        setIsLoading(true);
        setError(null);

        try {
            // Zakładam, że masz endpoint do aktualizacji usera, np. PUT /api/Account/UpdateProfile
            // lub używasz tego samego co przy finalizacji. Tutaj przykład generyczny:
            const response = await fetch('/api/User/UpdateProfile', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${user.token}`
                },
                body: JSON.stringify({
                    earnings: parseFloat(formData.earnings),
                    birthDate: formData.birthDate, // Backend musi obsłużyć string daty
                    jobStartDate: formData.jobStartDate,
                    jobEndDate: formData.jobEndDate || null // Pusty string zamieniamy na null
                })
            });

            if (!response.ok) {
                throw new Error("Nie udało się zapisać danych.");
            }

            // Aktualizujemy usera w kontekście aplikacji (bez przelogowania)
            const updatedUser = {
                ...user,
                earnings: parseFloat(formData.earnings),
                birthDate: formData.birthDate,
                jobStartDate: formData.jobStartDate,
                jobEndDate: formData.jobEndDate || null
            };
            login(updatedUser);

            alert("Profil zaktualizowany!");
            navigate('/search'); // Przekierowanie do kalkulatora

        } catch (err) {
            console.error(err);
            setError(err.message);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="card" style={{ maxWidth: '600px', margin: '40px auto', padding: '40px' }}>
            <h2 style={{ color: 'var(--brand2)', textAlign: 'center' }}>Uzupełnij Profil</h2>
            <p style={{ textAlign: 'center', color: '#aaa', marginBottom: '20px' }}>
                Aby korzystać z kalkulatora, musimy znać Twoją sytuację finansową.
            </p>

            {error && <p style={{ color: '#ff6b6b', textAlign: 'center' }}>{error}</p>}

            <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '20px' }}>
                
                <div style={{ display: 'flex', flexDirection: 'column' }}>
                    <label>Miesięczne zarobki netto (PLN)</label>
                    <input
                        type="number"
                        value={formData.earnings}
                        onChange={e => setFormData({ ...formData, earnings: e.target.value })}
                        required
                        min="0"
                        placeholder="np. 5000"
                        style={{ padding: '10px', background: '#222', border: '1px solid #444', color: '#fff', borderRadius: '5px' }}
                    />
                </div>

                <div style={{ display: 'flex', flexDirection: 'column' }}>
                    <label>Data urodzenia</label>
                    <input
                        type="date"
                        value={formData.birthDate}
                        onChange={e => setFormData({ ...formData, birthDate: e.target.value })}
                        required
                        style={{ padding: '10px', background: '#222', border: '1px solid #444', color: '#fff', borderRadius: '5px' }}
                    />
                </div>

                <div style={{ display: 'flex', flexDirection: 'column' }}>
                    <label>Początek obecnego zatrudnienia</label>
                    <input
                        type="date"
                        value={formData.jobStartDate}
                        onChange={e => setFormData({ ...formData, jobStartDate: e.target.value })}
                        required
                        style={{ padding: '10px', background: '#222', border: '1px solid #444', color: '#fff', borderRadius: '5px' }}
                    />
                </div>

                <div style={{ display: 'flex', flexDirection: 'column' }}>
                    <label>Koniec obecnego zatrudnienia (opcjonalne)</label>
                    <small style={{color: '#888', marginBottom: '5px'}}>Zostaw puste, jeśli umowa na czas nieokreślony</small>
                    <input
                        type="date"
                        value={formData.jobEndDate}
                        onChange={e => setFormData({ ...formData, jobEndDate: e.target.value })}
                        style={{ padding: '10px', background: '#222', border: '1px solid #444', color: '#fff', borderRadius: '5px' }}
                    />
                </div>

                <button 
                    type="submit" 
                    className="btn" 
                    style={{ marginTop: '10px', padding: '15px' }}
                    disabled={isLoading}
                >
                    {isLoading ? "Zapisywanie..." : "Zapisz i przejdź dalej"}
                </button>
            </form>
        </div>
    );
}

export default CompleteProfilePage;