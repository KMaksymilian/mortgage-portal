import { request } from './http';

export function getPendingOffers(token) {
  return request('/api/admin/offers/pending', { token });
}

export function approveOffer(token, offerId) {
  return request(`/api/admin/offers/${offerId}/approve`, { method: 'POST', token });
}

export function rejectOffer(token, offerId, reason) {
  return request(`/api/admin/offers/${offerId}/reject`, {
    method: 'POST',
    token,
    body: { reason },
  });
}
