import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { request } from './api/http';


function PublicCalculator() {
  const navigate = useNavigate();

  // Stany formularza
  const [formData, setFormData] = useState({ amount: '', months: '', ownContribution: '' });
  
  // ZMIANA: Zamiast 'result' (pojedynczy obiekt) mamy 'results' (tablica)
  const [results, setResults] = useState(null); 
  
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleCalculate = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    setResults(null);
    setError(null);

    const requestedAmountVal =
      Number(formData.amount) - (Number(formData.ownContribution) || 0);

    const payload = {
      bankName: '',
      id: 0,
      requestedAmount: { amount: requestedAmountVal, currencyCode: 'PLN' },
      installmentAmount: { amount: 0, currencyCode: 'PLN' },
      instalmentNumber: Number(formData.months),
      createdDate: new Date().toISOString(),
    };

    try {
        const data = await request('/api/Quote/PublicQuote', {
          method: 'POST',
          body: payload,
        });
        // jeśli backend zwróci { quoteId, offers }
        const offersFromWrapper = data?.offers ?? data?.Offers;

        // jeśli backend zwróci bezpośrednio listę
        const offers = Array.isArray(data)
          ? data
          : Array.isArray(offersFromWrapper)
            ? offersFromWrapper
            : data
              ? [data]
              : [];

        if (data?.quoteId) {
          localStorage.setItem('currentQuoteId', data.quoteId);
        }

        if (offers.length === 0) {
          setError('Brak ofert dla podanych parametrów.');
        } else {
          setResults(offers);
        }

    } catch (err) {
        console.error(err);
        setError("Nie udało się pobrać ofert. Sprawdź parametry.");
    } finally {
        setIsLoading(false);
    }
  };

  return (
    <div className="card" style={{ maxWidth: '900px', margin: '40px auto', padding: '40px', border: '1px solid var(--border)' }}>
      <div style={{ textAlign: 'center', marginBottom: '30px' }}>
        <h2 style={{ color: 'var(--brand2)', marginBottom: '10px' }}>
          Wyszukaj szacunkową ratę kredytu
        </h2>
        <p style={{ color: 'var(--muted)', fontSize: '0.95em' }}>
          Porównanie ofert z dostępnych banków (bez logowania).
        </p>
      </div>

      <form onSubmit={handleCalculate} style={{ 
          display: 'flex', gap: '20px', flexWrap: 'wrap', justifyContent: 'center', marginBottom: '30px'
      }}>
        {/* ... INPUTY BEZ ZMIAN ... */}
        <div style={{display: 'flex', flexDirection: 'column', gap: '8px'}}>
          <label style={{fontSize: '0.85em', color: 'var(--text)', fontWeight: 'bold'}}>Kwota kredytu</label>
          <input
            type="number" name="amount" value={formData.amount} onChange={handleChange} required min="1000" placeholder="np. 300000"
            style={{ padding: '12px', width: '160px', borderRadius: '8px', border: '1px solid var(--border)', backgroundColor: 'rgba(0,0,0,0.2)', color: 'white' }}
          />
        </div>

        <div style={{display: 'flex', flexDirection: 'column', gap: '8px'}}>
          <label style={{fontSize: '0.85em', color: 'var(--text)', fontWeight: 'bold'}}>Okres (miesiące)</label>
          <input
            type="number" name="months" value={formData.months} onChange={handleChange} required min="12" max="360" placeholder="np. 360"
            style={{ padding: '12px', width: '120px', borderRadius: '8px', border: '1px solid var(--border)', backgroundColor: 'rgba(0,0,0,0.2)', color: 'white' }}
          />
        </div>

        <div style={{display: 'flex', flexDirection: 'column', gap: '8px'}}>
          <label style={{fontSize: '0.85em', color: 'var(--text)', fontWeight: 'bold'}}>Wkład własny</label>
          <input
            type="number" name="ownContribution" value={formData.ownContribution} onChange={handleChange} min="0" placeholder="0"
            style={{ padding: '12px', width: '140px', borderRadius: '8px', border: '1px solid var(--border)', backgroundColor: 'rgba(0,0,0,0.2)', color: 'white' }}
          />
        </div>

        <div style={{ display: 'flex', alignItems: 'flex-end' }}>
          <button className="btn" type="submit" disabled={isLoading}>
            {isLoading ? 'Obliczanie...' : 'Oblicz'}
          </button>
        </div>
      </form>

      {error && <div style={{ color: '#ff6b6b', textAlign: 'center', marginBottom: '20px' }}>{error}</div>}

      {/* WYNIKI - LISTA OFERT */}
      {results && (
        <div style={{ display: 'flex', flexDirection: 'column', gap: '15px' }}>
           {results.map((offer, index) => {
               // Obsługa wielkości liter (zabezpieczenie)
              const installmentRaw =
                offer.installmentAmount?.amount ??
                offer.instalmentAmount?.amount ??
                offer.monthlyInstallment ??
                offer.MonthlyInstallment ??
                0;
              const installment = Number(installmentRaw) || 0;
              const amount = offer.amount ?? offer.Amount ?? offer.requestedAmount?.amount ?? 0;
              const currency =
                offer.installmentAmount?.currencyCode ??
                offer.requestedAmount?.currencyCode ??
                offer.currencyCode ??
                offer.currency ??
                'PLN';
              const bankName = offer.bankName || offer.BankName || `Bank #${index + 1}`;

               return (
                <div key={index} style={{ 
                    backgroundColor: 'rgba(255,255,255,0.03)', border: '1px solid var(--border)', borderRadius: '12px', padding: '20px',
                    animation: 'fadeIn 0.5s', display: 'flex', flexWrap: 'wrap', justifyContent: 'space-between', alignItems: 'center', gap: '20px'
                }}>
                    {/* Nazwa Banku */}
                    <div style={{ minWidth: '150px' }}>
                        <h3 style={{ margin: 0, color: 'var(--text)' }}>{bankName}</h3>
                    </div>

                    {/* Szczegóły Finansowe */}
                    <div style={{ textAlign: 'center' }}>
                        <span style={{ color: 'var(--muted)', fontSize: '0.8em', textTransform: 'uppercase' }}>Szacunkowa Rata</span>
                        <div style={{ fontSize: '1.8em', color: 'var(--brand2)', fontWeight: 'bold' }}>
                            {installment.toFixed(2)} <span style={{fontSize: '0.5em'}}>{currency}</span>
                        </div>
                    </div>

                    {/* Przycisk Akcji */}
                    <div>
                    <button 
                        onClick={() => {
                          const offerId = offer.internalId ?? offer.id ?? offer.offerId ?? null;

                          localStorage.setItem(
                            'pendingSelection',
                            JSON.stringify({
                              quoteId: localStorage.getItem('currentQuoteId') ?? null,
                              offerId,
                              bankName,
                            })
                          );

                          navigate('/login');
                        }}
                    >
                        Wybierz >
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

export default PublicCalculator;