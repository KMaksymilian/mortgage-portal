import { request } from './http';

export function quoteOffer(token, payload) {
  return request('/api/Offer/Quote', {
    method: 'POST',
    token,
    body: payload,
  });
}

export function acceptOffer(token, internalId) {
  // backend oczekuje [FromBody] int -> wysyłamy samą liczbę jako JSON
  return request('/api/Offer/accept', {
    method: 'POST',
    token,
    body: internalId,
  });
}

export function listOffers(token) {
  return request('/api/Offer', { token });
}

export function downloadContract(token, internalId) {
  // jeśli endpoint zwraca plik (PDF/doc) – bierzemy blob
  return request('/api/Offer/accept', {
    method: 'POST',
    token,
    body: internalId,
    responseType: 'blob',
  });
}
