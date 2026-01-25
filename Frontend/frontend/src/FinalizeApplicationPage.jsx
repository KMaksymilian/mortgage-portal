import { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from './AuthContext';

function FinalizeApplicationPage() {
    const { user, refreshUserData } = useAuth();
    const navigate = useNavigate();

    // --- STANY ---
    const [quoteId, setQuoteId] = useState(null);
    const [finalOffer, setFinalOffer] = useState(null); 
    
    const [formData, setFormData] = useState({
        earnings: user?.earnings || '',          
        birthDate: user?.birthDate ? user.birthDate.split('T')[0] : '',
        jobStartDate: user?.jobStartDate ? user.jobStartDate.split('T')[0] : '',
        jobEndDate: user?.jobEndDate ? user.jobEndDate.split('T')[0] : ''
    });

    const [isLoading, setIsLoading] = useState(false);
    const [isProcessing, setIsProcessing] = useState(false);
    const [error, setError] = useState(null);
    
    const autoSubmitRef = useRef(false);

    // --- 1. INICJALIZACJA ---
    useEffect(() => {
        if (finalOffer) return; 

        const storedId = localStorage.getItem('selectedQuoteId');
        if (!storedId) {
            navigate('/search');
            return;
        }
        setQuoteId(storedId);

        if (user && storedId && !autoSubmitRef.current) {
            const hasEarnings = user.earnings && user.earnings > 0;
            const hasBirthDate = !!user.birthDate;
            const hasJobStart = !!user.jobStartDate;

            if (hasEarnings && hasBirthDate && hasJobStart) {
                console.log("Dane kompletne - auto pobieranie...");
                autoSubmitRef.current = true; 
                fetchOfferAuto(storedId, user);
            }
        }
    // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [user, navigate, finalOffer]);

    // --- FUNKCJE API ---
    const fetchOfferAuto = async (qId, userData) => {
        setIsLoading(true);
        try {
            const payload = {
                quoteId: parseInt(qId),
                earnings: parseFloat(userData.earnings),
                birthDate: userData.birthDate,
                jobStartDate: userData.jobStartDate,
                jobEndDate: userData.jobEndDate || null
            };
            await sendRequest(payload);
        } catch (err) {
            console.error(err);
            setError("Nie udało się pobrać oferty automatycznie. Zweryfikuj dane.");
        } finally {
            setIsLoading(false);
        }
    };

    const handleSubmitManual = async (e) => {
        e.preventDefault();
        setIsLoading(true);
        setError(null);
        const payload = {
            quoteId: parseInt(quoteId),
            earnings: parseFloat(formData.earnings),
            birthDate: formData.birthDate,
            jobStartDate: formData.jobStartDate,
            jobEndDate: formData.jobEndDate || null
        };
        await sendRequest(payload);
        setIsLoading(false);
    };

    const sendRequest = async (payload) => {
        try {
            const response = await fetch('/api/Quote/Finalize', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${user.token}`
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                const msg = await response.text();
                throw new Error(msg || "Błąd pobierania oferty");
            }

            const offerData = await response.json();
            setFinalOffer(offerData);
            
            if (refreshUserData) await refreshUserData();
        } catch (err) {
            setError(err.message);
            throw err;
        }
    };

    // --- AKCJA AKCEPTACJI ---
    const handleAccept = async () => {
        if (!window.confirm("Czy akceptujesz warunki finansowe? Umowa zostanie wygenerowana w sekcji Historia.")) return;
        setIsProcessing(true);
        try {
            const response = await fetch(`/api/Offer/${finalOffer.id}/Accept`, {
                method: 'PUT',
                headers: { 'Authorization': `Bearer ${user.token}` }
            });

            if (!response.ok) throw new Error("Błąd akceptacji");

            localStorage.removeItem('selectedQuoteId');
            alert("Warunki zaakceptowane! Przejdź do historii, aby podpisać umowę.");
            navigate('/history');

        } catch (err) {
            alert(err.message);
        } finally {
            setIsProcessing(false);
        }
    };

    // --- AKCJA ODRZUCENIA (NOWA) ---
    const handleReject = async () => {
        if (!window.confirm("Czy na pewno chcesz odrzucić tę propozycję banku?")) return;
        setIsProcessing(true);
        
        try {
            // Wywołujemy endpoint odrzucania (żeby zmienić status w bazie)
            const response = await fetch(`/api/Offer/${finalOffer.id}/Reject`, {
                method: 'PUT', // lub DELETE, zależnie jak zrobisz w backendzie
                headers: { 'Authorization': `Bearer ${user.token}` }
            });

            if (!response.ok) throw new Error("Błąd odrzucania");

            localStorage.removeItem('selectedQuoteId');
            alert("Oferta odrzucona.");
            navigate('/search'); // Wracamy do szukania

        } catch (err) {
            alert(err.message);
        } finally {
            setIsProcessing(false);
        }
    };

    // --- WIDOKI ---
    if (isLoading && !finalOffer) {
        return (
            <div className="card" style={{textAlign: 'center', padding: '50px'}}>
                <h3 style={{color: 'var(--brand)'}}>Pobieranie ostatecznej oferty...</h3>
                <p>Bank analizuje Twoje dane finansowe.</p>
            </div>
        );
    }

    if (finalOffer) {
        const monthlyInstallment = finalOffer.monthlyInstallment?.amount || 0;
        const currency = finalOffer.monthlyInstallment?.currency || 'PLN';
        const loanAmount = finalOffer.requestedMoney?.amount || 0;
        const percentage = finalOffer.bankPercentage ? `${finalOffer.bankPercentage.toFixed(2)}%` : '---';
        
        return (
            <div className="card" style={{ maxWidth: '900px', margin: '40px auto', padding: '20px' }}>
                <h2 style={{textAlign: 'center', color: '#4CAF50', marginBottom: '10px'}}>Otrzymano ofertę wiążącą</h2>
                <p style={{textAlign: 'center', color: '#aaa', marginBottom: '30px'}}>
                    Bank <strong>{finalOffer.bankName}</strong> potwierdził następujące warunki. 
                    <br/>Zaakceptuj je, aby otrzymać umowę.
                </p>

                <div style={{ 
                    backgroundColor: '#1e1e1e', color: '#ffffff', 
                    border: '1px solid #4CAF50', borderRadius: '12px', padding: '30px',
                    display: 'flex', flexWrap: 'wrap', justifyContent: 'space-between', alignItems: 'center', gap: '25px',
                    boxShadow: '0 8px 20px rgba(76, 175, 80, 0.2)'
                }}>
                    
                    {/* Sekcja 1: Finanse */}
                    <div style={{ flex: '2 1 300px' }}>
                        <div style={{fontSize: '0.85em', color: '#888', marginBottom: '10px', textTransform: 'uppercase'}}>
                            SZCZEGÓŁY OFERTY (ID: {finalOffer.id})
                        </div>
                        
                        <div style={{ display: 'flex', gap: '40px', flexWrap: 'wrap' }}>
                            <div>
                                <span style={{ display: 'block', color: '#aaa', fontSize: '0.9em', marginBottom: '5px' }}>Ostateczna rata</span>
                                <div style={{ fontSize: '2em', fontWeight: 'bold', color: '#4CAF50' }}>
                                    {monthlyInstallment.toFixed(2)} <span style={{fontSize: '0.5em', color: '#fff'}}>{currency}</span>
                                </div>
                            </div>
                            <div>
                                <span style={{ display: 'block', color: '#aaa', fontSize: '0.9em', marginBottom: '5px' }}>Kwota kredytu</span>
                                <div style={{ fontSize: '1.4em', fontWeight: 'bold', marginTop: '5px', color: '#ddd' }}>
                                    {loanAmount.toFixed(0)} <span style={{fontSize: '0.6em'}}>{currency}</span>
                                </div>
                            </div>
                        </div>
                        
                        {/* UWAGA: Usunąłem link do PDF. Umowa pojawi się dopiero po kliknięciu Akceptuj w Historii. */}
                    </div>

                    {/* Sekcja 2: Oprocentowanie */}
                    <div style={{ 
                        display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center',
                        backgroundColor: '#2b2b2b', borderRadius: '8px', border: '1px solid #444', 
                        padding: '15px 25px', minWidth: '120px'
                    }}>
                         <span style={{ display: 'block', color: '#bbb', fontSize: '0.75em', textTransform: 'uppercase', marginBottom: '5px' }}>Oprocentowanie</span>
                         <strong style={{ fontSize: '1.6em', color: '#fff' }}>{percentage}</strong>
                    </div>

                    {/* Sekcja 3: Przyciski Akcji */}
                    <div style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                        <button 
                            onClick={handleAccept}
                            disabled={isProcessing}
                            style={{ 
                                backgroundColor: isProcessing ? '#888' : '#4CAF50', 
                                color: '#fff', fontWeight: 'bold', padding: '15px 40px', 
                                border: 'none', borderRadius: '50px', cursor: isProcessing ? 'not-allowed' : 'pointer',
                                fontSize: '1em', boxShadow: '0 4px 15px rgba(76, 175, 80, 0.3)'
                            }}
                        >
                            {isProcessing ? 'Przetwarzanie...' : 'Akceptuj warunki >'}
                        </button>
                        
                        <button 
                            onClick={handleReject}
                            disabled={isProcessing}
                            style={{ 
                                background: 'transparent', color: '#aaa', border: '1px solid #444', 
                                padding: '10px', borderRadius: '50px', cursor: 'pointer',
                                transition: 'color 0.2s, border-color 0.2s'
                            }}
                            onMouseOver={e => !isProcessing && (e.target.style.borderColor = '#ff6b6b') && (e.target.style.color = '#ff6b6b')}
                            onMouseOut={e => !isProcessing && (e.target.style.borderColor = '#444') && (e.target.style.color = '#aaa')}
                        >
                            Odrzuć ofertę
                        </button>
                    </div>

                </div>
            </div>
        );
    }

    return (
        <div className="card" style={{ maxWidth: '600px', margin: '40px auto', padding: '40px' }}>
            <h2 style={{color: 'var(--brand2)', textAlign: 'center'}}>Dokończ wniosek</h2>
            {error && <p style={{color: '#ff6b6b', textAlign: 'center'}}>{error}</p>}
            
            <form onSubmit={handleSubmitManual} style={{ display: 'flex', flexDirection: 'column', gap: '20px' }}>
                <div style={{display: 'flex', flexDirection: 'column'}}>
                    <label>Miesięczne zarobki netto</label>
                    <input 
                        type="number" value={formData.earnings} onChange={e => setFormData({...formData, earnings: e.target.value})} required 
                        style={{padding: '10px', background: '#222', border: '1px solid #444', color: '#fff', borderRadius: '5px'}}
                    />
                </div>
                {/* Reszta inputów bez zmian... */}
                <div style={{display: 'flex', flexDirection: 'column'}}>
                    <label>Data urodzenia</label>
                    <input type="date" value={formData.birthDate} onChange={e => setFormData({...formData, birthDate: e.target.value})} required style={{padding: '10px', background: '#222', border: '1px solid #444', color: '#fff', borderRadius: '5px'}}/>
                </div>
                <div style={{display: 'flex', flexDirection: 'column'}}>
                    <label>Początek zatrudnienia</label>
                    <input type="date" value={formData.jobStartDate} onChange={e => setFormData({...formData, jobStartDate: e.target.value})} required style={{padding: '10px', background: '#222', border: '1px solid #444', color: '#fff', borderRadius: '5px'}}/>
                </div>
                <div style={{display: 'flex', flexDirection: 'column'}}>
                    <label>Koniec zatrudnienia</label>
                    <input type="date" value={formData.jobEndDate} onChange={e => setFormData({...formData, jobEndDate: e.target.value})} style={{padding: '10px', background: '#222', border: '1px solid #444', color: '#fff', borderRadius: '5px'}}/>
                </div>

                <button type="submit" className="btn" style={{marginTop: '10px'}}>Sprawdź ofertę</button>
            </form>
        </div>
    );
}

export default FinalizeApplicationPage;