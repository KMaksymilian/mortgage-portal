import React from 'react';
import { useAuth } from './AuthContext';
import { useNavigate } from 'react-router-dom';
import PublicCalculator from './PublicCalculator'; // Importujemy nowy komponent

function HomePage() {
  const { user } = useAuth();
  const navigate = useNavigate();

  return (
    <div style={{ textAlign: 'center', paddingBottom: '40px' }}>
      
      {/* Sekcja Hero - Powitanie */}
      <div style={{ marginBottom: '50px', padding: '50px 20px' }}>
        <h1 style={{ 
            fontSize: '3.5em', 
            fontWeight: '800',
            background: 'linear-gradient(90deg, var(--brand), var(--brand2))', 
            WebkitBackgroundClip: 'text', 
            WebkitTextFillColor: 'transparent',
            marginBottom: '20px',
            lineHeight: '1.1'
        }}>
          Znajdź kredyt najtaniej jak to możliwe
        </h1>
        <p style={{ fontSize: '1.2em', color: 'var(--muted)', maxWidth: '650px', margin: '0 auto 30px', lineHeight: '1.6' }}>
          Porównujemy oferty z wielu banków. <br/>
          Oblicz ratę kredytową i podpisz umowę bez wychodzenia z domu.<br/>
          Zaloguj się, aby otrzymać spersonalizowaną ofertę
        </p>
        
        {!user && (
          <button 
            onClick={() => navigate('/login')}
            className="btn"
            style={{ 
                padding: '12px 35px', 
                fontSize: '1.1em', 
                background: 'var(--brand)', 
                borderColor: 'var(--brand)',
                color: 'white',
                boxShadow: '0 0 20px rgba(124,58,237,0.4)'
            }}
          >
            Zaloguj się
          </button>
        )}
      </div>

      {/* Sekcja Kalkulatora Publicznego */}
      <div id="calculator-preview" style={{position: 'relative', zIndex: 1}}>
        <PublicCalculator />
      </div>

    </div>
  );
}

// Mały komponent pomocniczy do kafelków
function InfoCard({ title, desc }) {
    return (
        <div className="card" style={{ width: '280px', padding: '30px', textAlign: 'left', background: 'rgba(255,255,255,0.03)' }}>
            <h3 style={{ color: 'white', marginTop: 0, marginBottom: '10px' }}>{title}</h3>
            <p style={{ color: 'var(--muted)', fontSize: '0.9em', lineHeight: '1.5' }}>{desc}</p>
        </div>
    );
}

export default HomePage;