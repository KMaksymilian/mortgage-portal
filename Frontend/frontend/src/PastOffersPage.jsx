import { useState, useEffect } from 'react';
import { useAuth } from './AuthContext';
import { useNavigate } from 'react-router-dom';

function UserHistoryPage() {
    const { user } = useAuth();
    const navigate = useNavigate();
    
    const [offers, setOffers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [downloadingId, setDownloadingId] = useState(null); // Do obsługi spinnera na przycisku

    // 1. POBIERANIE LISTY OFERT (GET)
    // To pobiera listę ofert, ale BEZ zawartości plików (żeby było szybko)
    useEffect(() => {
        if (!user) {
            navigate('/login');
            return;
        }

        const fetchOffers = async () => {
            try {
                const response = await fetch('http://localhost:5254/api/Offer', {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${user.token}`,
                        'Content-Type': 'application/json'
                    }
                });

                if (response.ok) {
                    const data = await response.json();
                    setOffers(data);
                } else {
                    console.error("Błąd pobierania historii");
                }
            } catch (error) {
                console.error("Błąd sieci:", error);
            } finally {
                setLoading(false);
            }
        };

        fetchOffers();
    }, [user, navigate]);

    // 2. FUNKCJA POBIERANIA PLIKU (To wywołuje Twój endpoint POST /accept)
const handleDownloadContract = async (offerId) => {
    setDownloadingId(offerId);

    try {
        const response = await fetch('http://localhost:5254/api/Offer/accept', {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${user.token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(offerId)
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || "Błąd pobierania pliku");
        }

        const blob = await response.blob();

        // === ZMIANA 1: Typ MIME ===
        // Ustawiamy text/plain, żeby przeglądarka wiedziała, że to tekst
        const newBlob = new Blob([blob], { type: 'text/plain' });

        const url = window.URL.createObjectURL(newBlob);
        const link = document.createElement('a');
        link.href = url;
        
        // === ZMIANA 2: Rozszerzenie pliku ===
        link.setAttribute('download', `Umowa_Oferta_${offerId}.txt`); 
        
        document.body.appendChild(link);
        link.click();
        
        // Sprzątanie
        link.parentNode.removeChild(link);
        window.URL.revokeObjectURL(url);

    } catch (error) {
        alert(`Nie udało się pobrać umowy: ${error.message}`);
    } finally {
        setDownloadingId(null);
    }
};

    if (loading) return <div style={{padding:'40px', textAlign:'center', color:'white'}}>Ładowanie historii...</div>;

    return (
        <div className="card" style={{ maxWidth: '1000px', margin: '40px auto', padding: '20px', backgroundColor: '#1e1e1e', color: '#fff', border: '1px solid #333' }}>
            <h2 style={{ textAlign: 'center', marginBottom: '30px', color: '#4CAF50' }}>Twoje Wnioski Kredytowe</h2>

            {offers.length === 0 ? (
                <p style={{ textAlign: 'center', color: '#aaa' }}>Nie masz jeszcze żadnych ofert.</p>
            ) : (
                <div style={{ overflowX: 'auto' }}>
                    <table style={{ width: '100%', borderCollapse: 'collapse', minWidth: '600px' }}>
                        <thead>
                            <tr style={{ borderBottom: '2px solid #444', textAlign: 'left', color: '#888' }}>
                                <th style={{ padding: '30px' }}>Data utworzenia</th>
                                <th style={{ padding: '30px' }}>Kwota</th>
                                <th style={{ padding: '30px' }}>Rata</th>
                                <th style={{ padding: '30px' }}>Status</th>
                                <th style={{ padding: '30px', textAlign: 'center' }}>Dokumenty</th>
                                <th stle={{padding: '30px', textAling: 'cented'}}>Podpisz umowę</th>
                            </tr>
                        </thead>
                        <tbody>
                            {offers.map((offer) => (
                                <tr key={offer.id} style={{ borderBottom: '1px solid #333' }}>
                                    <td style={{ padding: '15px' }}>
                                        {new Date(offer.createDate).toLocaleDateString()}
                                    </td>
                                    <td style={{ padding: '15px', fontWeight: 'bold' }}>
                                        {offer.loanAmount} {offer.currencycode}
                                    </td>
                                    <td style={{ padding: '15px' }}>
                                        {/* Zakładam, że w GET /Offers zwracasz to pole, jeśli nie - usuń */}
                                        {offer.monthlyInstallment ? `${offer.monthlyInstallment} ${offer.currencycode}` : '-'}
                                    </td>
                                    <td style={{ padding: '15px' }}>
                                        <span style={{ 
                                            color: offer.status === 'ReadyToBeSigned' ? '#4CAF50' : '#aaa',
                                            border: `1px solid ${offer.status === 'ReadyToBeSigned' ? '#4CAF50' : '#555'}`,
                                            padding: '4px 8px',
                                            borderRadius: '4px',
                                            fontSize: '0.85em'
                                        }}>
                                            {offer.status}
                                        </span>
                                    </td>
                                    <td style={{ padding: '15px', textAlign: 'center' }}>
                                        <button 
                                            onClick={() => handleDownloadContract(offer.id)}
                                            disabled={downloadingId === offer.id}
                                            style={{
                                                backgroundColor: '#2196F3',
                                                color: 'white',
                                                border: 'none',
                                                padding: '8px 20px',
                                                borderRadius: '30px',
                                                cursor: downloadingId === offer.id ? 'wait' : 'pointer',
                                                opacity: downloadingId === offer.id ? 0.7 : 1,
                                                fontWeight: 'bold',
                                                fontSize: '0.9em',
                                                transition: 'transform 0.1s'
                                            }}
                                            onMouseOver={(e) => downloadingId !== offer.id && (e.target.style.transform = 'scale(1.05)')}
                                            onMouseOut={(e) => downloadingId !== offer.id && (e.target.style.transform = 'scale(1)')}
                                        >
                                            {offer.Completed ? "Podpisana" : "Pobierz umowę"}
                                        </button>
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

export default UserHistoryPage;