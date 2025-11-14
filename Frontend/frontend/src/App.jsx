import { useState, useEffect } from 'react';
import './App.css'; // Możesz zostawić lub usunąć ten import

function App() {
  // Tworzymy "stan" do przechowywania wiadomości z API
  const [message, setMessage] = useState('');

  // useEffect uruchamia kod, gdy komponent się załaduje
  useEffect(() => {
    // To jest zapytanie do Twojego backendu (API)
    fetch('http://localhost:5254/api/home') // <-- WAŻNE: Sprawdź ten port!
      .then(response => response.json())
      .then(data => {
        // Ustawiamy wiadomość w stanie, gdy dane nadejdą
        setMessage(data.message); 
      })
      .catch(error => {
        console.error("Błąd pobierania danych:", error);
        setMessage("Nie udało się połączyć z API :(");
      });
  }, []); // Pusta tablica [] oznacza "uruchom to tylko raz"

  // To jest HTML (JSX), który zobaczy użytkownik
  return (
    <div className="App">
      <h1>Moja aplikacja React</h1>
      <p>Wiadomość z backendu (.NET):</p>
      <h2>{message}</h2>
    </div>
  );
}

export default App;