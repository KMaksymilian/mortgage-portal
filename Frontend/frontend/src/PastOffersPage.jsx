import { useState, useEffect, useRef } from 'react';
import { useAuth } from './AuthContext';
import { useNavigate } from 'react-router-dom';
import { downloadContract, uploadSignedContract } from './api/offers';


function UserHistoryPage() {
    const { user } = useAuth();
    const navigate = useNavigate();

    const fileInputRef = useRef(null);
    
    const [offers, setOffers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [downloadingId, setDownloadingId] = useState(null);
    const [uploadingId, setUploadingId] = useState(null);

    useEffect(() => {
        if (!user) { navigate('/login'); return; }
        fetchOffers();
    }, [user, navigate]);

const fetchOffers = async () => {
        try {
            const response = await fetch('/api/Offer', {
                method: 'GET',
                headers: { 'Authorization': `Bearer ${user.token}` }
            });
            if (response.ok) {
                const data = await response.json();
                setOffers(data);
            }
        } catch (error) {
            console.error("Błąd sieci:", error);
        } finally {
            setLoading(false);
        }
    };

    // --- FUNKCJA POBIERANIA UMOWY ---
    const handleDownloadContract = async (offerId, fileNameFromOffer) => {
        setDownloadingId(offerId);

        try {
            // Celujemy w endpoint: [HttpGet("{offerId}/Download")]
            const response = await fetch(`/api/Offer/${offerId}/Download`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${user.token}`
                    // Content-Type zbędny przy GET
                }
            });

            if (!response.ok) {
                // Jeśli 404, to znaczy że nie ma umowy
                if (response.status === 404) {
                    throw new Error("Umowa nie została jeszcze wygenerowana lub nie istnieje.");
                }
                const errorText = await response.text();
                throw new Error(errorText || "Błąd pobierania pliku");
            }

            // Pobieramy Blob (plik)
            const blob = await response.blob();
            
            // Tworzymy link
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            
            // Próbujemy ustalić nazwę pliku:
            // 1. Z bazy (jeśli przekazana)
            // 2. Domyślna
            const fileName = fileNameFromOffer || `Umowa_Oferta_${offerId}.txt`;
            link.setAttribute('download', fileName);
            
            document.body.appendChild(link);
            link.click();

            // Sprzątanie
            link.parentNode.removeChild(link);
            window.URL.revokeObjectURL(url);

        } catch (error) {
            console.error(error);
            alert(`Nie udało się pobrać: ${error.message}`);
        } finally {
            setDownloadingId(null);
        }
    };

        const handleSignClick = (offerId) => {
        setUploadingId(offerId); // Zapamiętujemy ID oferty, którą chcemy podpisać
        if (fileInputRef.current) {
            fileInputRef.current.value = ''; // Czyścimy input
            fileInputRef.current.click();    // Symulujemy kliknięcie w input
        }
    };
const handleFileChange = async (event) => {
        const file = event.target.files[0];
        
        // Jeśli anulowano wybór
        if (!file) {
            setUploadingId(null);
            return;
        }

        // Walidacja po stronie frontendu (żeby nie męczyć serwera)
        if (!file.name.endsWith('.txt')) {
            alert("Dozwolone są tylko pliki tekstowe (.txt).");
            setUploadingId(null);
            return;
        }

        if (!window.confirm(`Wysłać plik "${file.name}" jako podpis?`)) {
            setUploadingId(null);
            return;
        }

        try {
            // Wywołanie funkcji z api/offers.js
            await uploadSignedContract(user.token, uploadingId, file);
            
            alert("Umowa podpisana pomyślnie!");
            // Odświeżamy listę, żeby zaktualizować status na 'Completed'
            fetchOffers();

        } catch (error) {
            console.error(error);
            alert(`Nie udało się podpisać umowy: ${error.message}`);
        } finally {
            setUploadingId(null);
        }
    };

    if (loading) return <div style={{padding:'40px', textAlign:'center', color:'white'}}>Ładowanie...</div>;

    return (
        <div className="card" style={{ maxWidth: '1000px', margin: '40px auto', padding: '20px', backgroundColor: '#1e1e1e', color: '#fff', border: '1px solid #333' }}>
            <h2 style={{ textAlign: 'center', marginBottom: '30px', color: '#4CAF50' }}>Twoje Wnioski Kredytowe</h2>

            {/* UKRYTY INPUT - Służy do otwierania okna plików */}
            <input 
                type="file" 
                ref={fileInputRef} 
                style={{ display: 'none' }} 
                accept=".txt" 
                onChange={handleFileChange} 
            />

            {offers.length === 0 ? (
                <p style={{ textAlign: 'center', color: '#aaa' }}>Brak ofert.</p>
            ) : (
                <div style={{ overflowX: 'auto' }}>
                    <table style={{ width: '100%', borderCollapse: 'collapse', minWidth: '600px' }}>
                        <thead>
                            <tr style={{ borderBottom: '2px solid #444', textAlign: 'left', color: '#888' }}>
                                <th style={{ padding: '20px' }}>Data</th>
                                <th style={{ padding: '20px' }}>Kwota</th>
                                <th style={{ padding: '20px' }}>Status</th>
                                <th style={{ padding: '20px', textAlign: 'center' }}>Akcje</th>
                            </tr>
                        </thead>
                        <tbody>
                            {offers.map((offer) => {
                                // Warunek: Pokaż przycisk "Podpisz", jeśli oferta jest Approved (zaakceptowana), ale nie Completed
                                const canSign = offer.status === 'Approved';
                                const isCompleted = offer.status === 'Completed';

                                return (
                                    <tr key={offer.id} style={{ borderBottom: '1px solid #333' }}>
                                        <td style={{ padding: '15px' }}>{new Date(offer.createDate).toLocaleDateString()}</td>
                                        <td style={{ padding: '15px', fontWeight: 'bold' }}>{offer.loanAmount} {offer.currencycode}</td>
                                        <td style={{ padding: '15px' }}>
                                            <span style={{ 
                                                color: isCompleted ? '#4CAF50' : '#aaa',
                                                border: `1px solid ${isCompleted ? '#4CAF50' : '#555'}`,
                                                padding: '4px 8px', borderRadius: '4px', fontSize: '0.85em'
                                            }}>
                                                {offer.status}
                                            </span>
                                        </td>
                                        
                                        <td style={{ padding: '15px', display: 'flex', gap: '10px', justifyContent: 'center' }}>
                                            {/* PRZYCISK POBIERZ */}
                                            <button 
                                                onClick={() => handleDownloadContract(offer.id, offer.fileName)}
                                                disabled={downloadingId === offer.id}
                                                style={{
                                                    backgroundColor: '#2196F3', color: 'white', border: 'none',
                                                    padding: '8px 15px', borderRadius: '20px', cursor: 'pointer',
                                                    opacity: downloadingId === offer.id ? 0.7 : 1
                                                }}
                                            >
                                                {downloadingId === offer.id ? '...' : 'Pobierz'}
                                            </button>

                                            {/* PRZYCISK PODPISZ */}
                                            {canSign && (
                                                <button 
                                                    onClick={() => handleSignClick(offer.id)}
                                                    disabled={uploadingId === offer.id}
                                                    style={{
                                                        backgroundColor: '#e91e63', color: 'white', border: 'none',
                                                        padding: '8px 15px', borderRadius: '20px', cursor: 'pointer',
                                                        fontWeight: 'bold',
                                                        opacity: uploadingId === offer.id ? 0.7 : 1
                                                    }}
                                                >
                                                    {uploadingId === offer.id ? 'Wysyłanie...' : 'Podpisz'}
                                                </button>
                                            )}
                                        </td>
                                    </tr>
                                );
                            })}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
}

export default UserHistoryPage;