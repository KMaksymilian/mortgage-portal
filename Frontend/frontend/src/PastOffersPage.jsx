import { useState, useEffect } from 'react';
import { useAuth } from './AuthContext';

function PastOffersPage() {
    const { user } = useAuth();
    const [offers, setOffers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchOffers = async () => {
            // Jeśli brak usera, przerywamy
            if (!user || !user.token) return;

            try {
                // WAŻNE: Upewnij się, że port (5254) jest zgodny z Twoim backendem
                const response = await fetch('http://localhost:5254/api/Offer', {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${user.token}`, // Token autoryzacji
                        'Content-Type': 'application/json'
                    }
                });

                if (!response.ok) {
                    throw new Error(`Błąd pobierania ofert: ${response.status}`);
                }

                const data = await response.json();
                setOffers(data);

            } catch (err) {
                console.error(err);
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };

        fetchOffers();
    }, [user]);

    if (loading) return <div className="card"><p>Ładowanie historii ofert...</p></div>;
    if (error) return <div className="card" style={{ color: 'red' }}><p>Wystąpił błąd: {error}</p></div>;

    return (
        <div className="card">
            <h2>Twoja historia zapytań</h2>
            
            {offers.length === 0 ? (
                <p>Nie masz jeszcze żadnych zapisanych ofert.</p>
            ) : (
                <div style={{ overflowX: 'auto' }}>
                    <table style={{ width: '100%', borderCollapse: 'collapse', marginTop: '20px', minWidth: '600px' }}>
                        <thead>
                            <tr style={{ backgroundColor: '#333', color: '#fff' }}>
                                <th style={{ padding: '10px', textAlign: 'left' }}>Data utworzenia</th>
                                <th style={{ padding: '10px', textAlign: 'left' }}>Kwota</th>
                                <th style={{ padding: '10px', textAlign: 'left' }}>Status</th>
                                <th style={{ padding: '10px', textAlign: 'left' }}>ID Oferty (QuoteId)</th>
                            </tr>
                        </thead>
                        <tbody>
                            {offers.map((offer) => (
                                <tr key={offer.id} style={{ borderBottom: '1px solid #444' }}>
                                    <td style={{ padding: '10px' }}>
                                        {new Date(offer.date).toLocaleDateString()} {new Date(offer.date).toLocaleTimeString()}
                                    </td>
                                    <td style={{ padding: '10px', fontWeight: 'bold', color: '#646cff' }}>
                                        {offer.amount} {offer.currency}
                                    </td>
                                    <td style={{ padding: '10px' }}>
                                        {/* Proste stylowanie statusu */}
                                        <span style={{ 
                                            padding: '4px 8px', 
                                            borderRadius: '4px', 
                                            backgroundColor: offer.status === 'Created' ? '#555' : 'green',
                                            fontSize: '0.9em'
                                        }}>
                                            {offer.status}
                                        </span>
                                    </td>
                                    <td style={{ padding: '10px', fontSize: '0.85em', color: '#aaa' }}>
                                        {offer.quoteId}
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
}

export default PastOffersPage;