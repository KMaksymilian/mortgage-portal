import { useState } from 'react';
import { useAuth } from './AuthContext';

function OfferSearchPage() {
  const { user } = useAuth(); 
  
  // Stan formularza
  const [formData, setFormData] = useState({
    amount: '',         // Kwota
    months: '',         // Ilość rat
    ownContribution: '' // Opcjonalnie
  });

  const [result, setResult] = useState(null);
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setResult(null);
    setIsLoading(true);

    if (!user) {
      setError("Musisz być zalogowany.");
      setIsLoading(false);
      return;
    }

    try {
      // --- TU JEST ZMIANA ---
      // Tworzymy strukturę idealnie pasującą do Twojego JSON-a
      const payload = {
        requestedAmount: {
          amount: parseFloat(formData.amount - formData.ownContribution), // Zamiana tekstu na liczbę
          currencyCode: "PLN"
        },
        instalmentNumber: parseInt(formData.months) // Zamiana tekstu na liczbę całkowitą
      };
      // ----------------------

      console.log("Wysyłam JSON:", JSON.stringify(payload, null, 2)); // Podgląd w konsoli

      const response = await fetch('http://localhost:5254/Quote', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${user.token}`
        },
        body: JSON.stringify(payload)
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Błąd API (${response.status}): ${errorText}`);
      }

      const data = await response.json();
      setResult(data);

    } catch (err) {
      console.error(err);
      setError(err.message);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="card">
      <h2>Kalkulator Oferty</h2>
      
      <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '10px', maxWidth: '400px', margin: '0 auto' }}>
        
        {/* Pole 1: Kwota */}
        <div style={{ textAlign: 'left' }}>
          <label>Kwota kredytu (PLN):</label>
          <input
            type="number"
            name="amount"
            value={formData.amount}
            onChange={handleChange}
            required
            min="100"
            step="0.01"
            style={{ width: '100%', padding: '8px', marginTop: '5px' }}
          />
        </div>

        {/* Pole 2: Ilość rat */}
        <div style={{ textAlign: 'left' }}>
          <label>Ilość rat (miesięcy):</label>
          <input
            type="number"
            name="months"
            value={formData.months}
            onChange={handleChange}
            required
            min="1"
            max="120"
            style={{ width: '100%', padding: '8px', marginTop: '5px' }}
          />
        </div>

        {/* Pole 3: Wkład własny (opcjonalne) */}
        <div style={{ textAlign: 'left' }}>
          <label>Wkład własny (opcjonalnie):</label>
          <input
            type="number"
            name="ownContribution"
            value={formData.ownContribution}
            onChange={handleChange}
            min="0"
            style={{ width: '100%', padding: '8px', marginTop: '5px' }}
          />
        </div>

        <button type="submit" disabled={isLoading} style={{ marginTop: '15px' }}>
          {isLoading ? 'Oblicz Raty' : 'Oblicz'}
        </button>
      </form>

      {/* Wyświetlanie błędów */}
      {error && <div style={{ color: 'red', marginTop: '20px', border: '1px solid red', padding: '10px' }}>{error}</div>}

      {/* Wyświetlanie wyniku */}
      {result && (
        <div style={{ marginTop: '30px', padding: '15px', border: '1px solid #646cff', borderRadius: '8px', textAlign: 'left' }}>
          <h3>Wynik Kalkulacji:</h3>
          
          {/* Dostosuj wyświetlanie do tego, co API zwraca w odpowiedzi */}
          <p>Miesięczna rata: <strong>{result.instalmentAmount?.amount ?? result.instalmentAmount.amount ?? '---'} {result.instalmentAmount.currencyCode}</strong></p>
          <p>Całkowity koszt: <strong>
      {/* Mnożymy kwotę raty z API przez liczbę miesięcy z formularza */}
      { (result.instalmentAmount?.amount * formData.months).toFixed(2)} {result.instalmentAmount?.currencyCode}
    </strong></p>

          <details style={{marginTop: '15px'}}>
            <summary style={{cursor: 'pointer'}}>Pokaż szczegóły techniczne (JSON)</summary>
            <pre>{JSON.stringify(result, null, 2)}</pre>
          </details>
        </div>
      )}
    </div>
  );
}

export default OfferSearchPage;