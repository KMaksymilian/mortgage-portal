import { request } from './http';

// POST /api/Quote/PublicQuote
export function publicQuote({ requestedAmount, currencyCode, instalmentNumber }) {
  return request('/api/Quote/PublicQuote', {
    method: 'POST',
    body: {
      bankName: 'public',
      id: 0,
      requestedAmount: { amount: requestedAmount, currencyCode },
      installmentAmount: { amount: 0, currencyCode },
      instalmentNumber,
      createdDate: new Date().toISOString(),
    },
  });
}
