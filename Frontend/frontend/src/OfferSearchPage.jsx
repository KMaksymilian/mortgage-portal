import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from './AuthContext';
import { acceptOffer } from './api/offers';
import { request } from './api/http';



function OfferSearchPage() {
  const { user } = useAuth();
  const navigate = useNavigate();
  
  // --- STANY ---
  const [formData, setFormData] = useState({ amount: '', months: '', ownContribution: '' });
  const [offersList, setOffersList] = useState(null);
  const [isOfferAccepted, setIsOfferAccepted] = useState(false);
  
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isProcessing, setIsProcessing] = useState(false);


  // Obsługa inputów formularza
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  // --- 1. WYSYŁANIE ZAPYTANIA O OFERTĘ (QUOTE) ---
  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setOffersList(null);
    setIsOfferAccepted(false);
    setIsLoading(true);

    try {
      // Obliczamy kwotę netto (kwota kredytu - wkład własny)
      const requestedAmountVal = parseFloat(formData.amount) - (parseFloat(formData.ownContribution) || 0);

      const payload = {
        bankName: "",        // QuoteDto ma to pole, może być puste
        id: 0,               // QuoteDto ma Id
        requestedAmount: { amount: requestedAmountVal, currencyCode: "PLN" },
        installmentAmount: { amount: 0, currencyCode: "PLN" }, // QuoteDto ma InstallmentAmount
        instalmentNumber: parseInt(formData.months, 10),
        createdDate: new Date().toISOString(),
      };

      const data = await request('/api/Quote/PublicQuote', {
        method: 'POST',
        body: payload,
      });

      if (Array.isArray(data)) {
          setOffersList(data);
      } else if (data) {
          // Jeśli backend zwróciłby jednak pojedynczy obiekt (np. błąd lub jedną ofertę bez tablicy)
          setOffersList([data]);
      } else {
          setOffersList([]);
}

    } catch (err) {
      console.error(err);
      setError(err.message);
    } finally {
      setIsLoading(false);
    }
  };

  // --- 2. AKCEPTACJA OFERTY ---
  const handleSelectOffer = async (internalId) => {
    
    if (!user) {
      alert("Zaloguj się, aby zaakceptować ofertę.");
      navigate('/login');
      return;
    }

    if (!window.confirm("Czy na pewno chcesz zaakceptować tę ofertę?")) return;
    setIsProcessing(true);
    setError(null);

    try {
        await acceptOffer(user.token, internalId);
        setIsOfferAccepted(true);
    } catch (err) {
        alert(`Wystąpił błąd: ${err.message}`);
    } finally {
        setIsProcessing(false);
    }
  };

  // --- WIDOK SUKCESU (Po akceptacji) ---
  if (isOfferAccepted) {
      return (
          <div className="card" style={{ maxWidth: '600px', margin: '40px auto', textAlign: 'center', padding: '40px', backgroundColor: '#1e1e1e', color: '#fff', border: '1px solid #4CAF50', borderRadius: '10px' }}>
              <h2 style={{ color: '#4CAF50', marginBottom: '10px' }}>Gratulacje!</h2>
              <p style={{ fontSize: '1.1em', color: '#ddd' }}>Twoja oferta została pomyślnie zaakceptowana.</p>
              <p style={{ color: '#aaa', fontSize: '0.9em', marginTop: '10px' }}>Możesz teraz przejść do historii swoich wniosków, gdzie czeka na Ciebie umowa do podpisania</p>
              
              <button 
                  onClick={() => { setIsOfferAccepted(false); navigate('/history'); }}
                  style={{ 
                      marginTop: '30px', 
                      background: 'transparent', 
                      border: '2px solid #4CAF50', 
                      color: '#4CAF50', 
                      padding: '10px 25px', 
                      cursor: 'pointer',
                      borderRadius: '5px',
                      fontWeight: 'bold',
                      fontSize: '1em'
                  }}
              >
                  Wróć do listy spraw
              </button>
          </div>
      );
  }

  // --- WIDOK GŁÓWNY (KALKULATOR) ---
  return (
    <div className="card" style={{ maxWidth: '900px', margin: '0 auto', padding: '20px' }}>
      <h2 style={{textAlign: 'center', marginBottom: '25px', color: '#fff'}}>Kalkulator Kredytowy</h2>
      
      {/* Formularz */}
      <form onSubmit={handleSubmit} style={{ 
          display: 'flex', gap: '20px', flexWrap: 'wrap', justifyContent: 'center', marginBottom: '40px',
          padding: '30px', backgroundColor: '#252525', borderRadius: '12px', border: '1px solid #333',
          boxShadow: '0 4px 10px rgba(0,0,0,0.3)'
      }}>
        <div style={{display: 'flex', flexDirection: 'column'}}>
          <label style={{fontSize: '0.85em', marginBottom: '8px', color: '#ccc', fontWeight: 'bold'}}>Kwota kredytu (PLN)</label>
          <input
            type="number" name="amount" value={formData.amount} onChange={handleChange} required min="100" placeholder="np. 50000"
            style={{ padding: '12px', width: '160px', borderRadius: '6px', border: '1px solid #555', backgroundColor: '#1a1a1a', color: 'white', fontSize: '1em' }}
          />
        </div>

        <div style={{display: 'flex', flexDirection: 'column'}}>
          <label style={{fontSize: '0.85em', marginBottom: '8px', color: '#ccc', fontWeight: 'bold'}}>Okres (miesiące)</label>
          <input
            type="number" name="months" value={formData.months} onChange={handleChange} required min="1" max="120" placeholder="np. 48"
            style={{ padding: '12px', width: '120px', borderRadius: '6px', border: '1px solid #555', backgroundColor: '#1a1a1a', color: 'white', fontSize: '1em' }}
          />
        </div>

        <div style={{display: 'flex', flexDirection: 'column'}}>
          <label style={{fontSize: '0.85em', marginBottom: '8px', color: '#ccc', fontWeight: 'bold'}}>Wkład własny (PLN)</label>
          <input
            type="number" name="ownContribution" value={formData.ownContribution} onChange={handleChange} min="0" placeholder="0"
            style={{ padding: '12px', width: '140px', borderRadius: '6px', border: '1px solid #555', backgroundColor: '#1a1a1a', color: 'white', fontSize: '1em' }}
          />
        </div>

        <div style={{ display: 'flex', alignItems: 'flex-end' }}>
             <button type="submit" disabled={isLoading} style={{ 
                 padding: '12px 35px', cursor: 'pointer', 
                 backgroundColor: isLoading ? '#555' : '#4CAF50', 
                 color: 'white', border: 'none', borderRadius: '6px', 
                 fontWeight: 'bold', fontSize: '1em',
                 transition: 'background 0.3s'
             }}>
                {isLoading ? 'Obliczanie...' : 'Oblicz ratę'}
             </button>
        </div>
      </form>

      {/* Wyświetlanie błędów */}
      {error && (
        <div style={{ 
            color: '#ff6b6b', marginBottom: '20px', textAlign: 'center', 
            padding: '15px', backgroundColor: 'rgba(255, 107, 107, 0.1)', 
            borderRadius: '8px', border: '1px solid #ff6b6b' 
        }}>
            <strong>Błąd:</strong> {error}
        </div>
      )}

      {/* Lista Ofert (Wyniki) */}
      {offersList && (
        <div style={{ display: 'flex', flexDirection: 'column', gap: '20px' }}>
          {offersList.map((offer, index) => {
              
              
              const monthlyInstallment =
                offer.installmentAmount?.amount ??
                offer.monthlyInstallment ??
                offer.monthlyInstallment?.amount ??
                0;

              const loanAmount =
                offer.requestedAmount?.amount ??
                offer.loanAmount ??
                offer.amount ??
                0;

              const currency =
                offer.installmentAmount?.currencyCode ??
                offer.requestedAmount?.currencyCode ??
                offer.currencyCode ??
                offer.currency ??
                'PLN';

              const percentageRaw = offer.percentage ?? offer.interestRate ?? offer.apr;
              const percentage = typeof percentageRaw === 'number' ? `${percentageRaw.toFixed(2)}%` : '---';

              const offerId = offer.internalId ?? offer.id ?? offer.offerId ?? index;
              const bankName = offer.bankName ?? offer.BankName ?? 'Bank';

              // Obliczenie sumy opłat (tylko do wyświetlenia szacunkowo)
              const months = parseInt(formData.months) || 1;
              const totalCost = (monthlyInstallment * months).toFixed(2);

              return (
                <div key={offerId} style={{ 
                    backgroundColor: '#1e1e1e', color: '#ffffff', 
                    border: '1px solid #333', borderRadius: '12px', padding: '30px',
                    display: 'flex', flexWrap: 'wrap', justifyContent: 'space-between', alignItems: 'center', gap: '25px',
                    boxShadow: '0 8px 20px rgba(0,0,0,0.4)'
                }}>
                    
                    {/* Sekcja 1: Szczegóły finansowe */}
                    <div style={{ flex: '2 1 300px' }}>
                        <div style={{fontSize: '0.85em', color: '#888', marginBottom: '10px', textTransform: 'uppercase', letterSpacing: '1px'}}>
                            Oferta wygenerowana — {bankName} (ID: {offerId})
                        </div>
                        
                        <div style={{ display: 'flex', gap: '40px', flexWrap: 'wrap' }}>
                            <div>
                                <span style={{ display: 'block', color: '#aaa', fontSize: '0.9em', marginBottom: '5px' }}>Rata miesięczna</span>
                                <div style={{ fontSize: '2em', fontWeight: 'bold', color: '#4CAF50' }}>
                                    {monthlyInstallment} <span style={{fontSize: '0.5em', color: '#fff'}}>{currency}</span>
                                </div>
                            </div>
                            
                            <div>
                                <span style={{ display: 'block', color: '#aaa', fontSize: '0.9em', marginBottom: '5px' }}>Kwota kredytu</span>
                                <div style={{ fontSize: '1.4em', fontWeight: 'bold', marginTop: '5px', color: '#ddd' }}>
                                    {loanAmount} <span style={{fontSize: '0.6em'}}>{currency}</span>
                                </div>
                            </div>

                            <div>
                                <span style={{ display: 'block', color: '#aaa', fontSize: '0.9em', marginBottom: '5px' }}>Całkowity koszt</span>
                                <div style={{ fontSize: '1.4em', fontWeight: 'bold', marginTop: '5px', color: '#ddd' }}>
                                    {totalCost} <span style={{fontSize: '0.6em'}}>{currency}</span>
                                </div>
                            </div>
                        </div>
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

                    {/* Sekcja 3: Przycisk Akcji */}
                    <div style={{ flexShrink: 0 }}>
                        <button 
                            onClick={() => handleSelectOffer(offer.internalId ?? offer.id ?? offer.offerId)}
                            disabled={isProcessing || !user}
                            title={!user ? 'Zaloguj się, aby zaakceptować ofertę' : ''}
                            style={{ 
                                backgroundColor: isProcessing ? '#888' : '#fff', 
                                color: '#000', 
                                fontWeight: 'bold', 
                                padding: '15px 40px', 
                                border: 'none', 
                                borderRadius: '50px', 
                                cursor: isProcessing ? 'not-allowed' : 'pointer',
                                fontSize: '1em',
                                boxShadow: '0 4px 15px rgba(255, 255, 255, 0.1)',
                                transition: 'transform 0.2s, box-shadow 0.2s'
                            }}
                            onMouseOver={(e) => !isProcessing && (e.target.style.transform = 'scale(1.05)')}
                            onMouseOut={(e) => !isProcessing && (e.target.style.transform = 'scale(1)')}
                        >
                            {!user ? 'Zaloguj się, aby zaakceptować' : (isProcessing ? 'Przetwarzanie...' : 'Akceptuj ofertę >')}
                        </button>
                    </div>

                </div>
              );
          })}
        </div>
      )}
    </div>
  );
}

export default OfferSearchPage;