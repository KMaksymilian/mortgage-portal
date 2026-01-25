import { request } from './http';

export function quoteOffer(token, payload) {
  return request('/api/Offer/Quote', {
    method: 'POST',
    token,
    body: payload,
  });
}

export const acceptOffer = async (token, offerId) => {
  // 1. URL: Bez parametrów query (czysty adres)
  const response = await fetch('/api/Offer/accept', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json' // <--- WYMAGANE przy [FromBody]
    },
    // 2. BODY: JSON.stringify samej liczby.
    // To wyśle po prostu: 15
    // Gdybyś wysłał { offerId: offerId }, backend odczytałby to jako błąd lub 0.
    body: JSON.stringify(offerId) 
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || 'Błąd akceptacji oferty');
  }

  return true; 
};

export function listOffers(token) {
  return request('/api/Offer', { token });
}

export const downloadContract = async (token, offerId) => {
  // Celujemy w endpoint: GET /api/Offer/15/Download
  const response = await fetch(`/api/Offer/${offerId}/Download`, {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${token}`
      // Nie ustawiamy Content-Type, bo to GET bez body
    }
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || 'Błąd pobierania pliku');
  }

  // Ważne: Zwracamy blob() (plik binarny), a nie json()
  return await response.blob();
};
// src/api/offers.js

export const uploadSignedContract = async (token, offerId, file) => {
  // 1. Tworzymy kontener na dane formularza
  const formData = new FormData();
  
  // 'file' musi pasować do nazwy parametru w kontrolerze ([FromForm] IFormFile file)
  formData.append('file', file); 

  // 2. Wysyłamy POST
  const response = await fetch(`/api/Offer/${offerId}/Sign`, {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`
      // WAŻNE: Nie ustawiaj tutaj Content-Type! 
      // Przeglądarka sama ustawi 'multipart/form-data' wraz z granicami (boundary).
    },
    body: formData
  });

  if (!response.ok) {
    const text = await response.text();
    // Próbujemy wyciągnąć komunikat JSON jeśli taki jest, w przeciwnym razie tekst
    try {
        const json = JSON.parse(text);
        throw new Error(json.message || json.title || text);
    } catch {
        throw new Error(text || 'Błąd przesyłania podpisanej umowy');
    }
  }

  return true;
};


