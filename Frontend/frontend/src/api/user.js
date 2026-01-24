import { request } from './http';

export function getMe(token) {
  return request('/api/User/Me', { token });
}

export function setBirthDate(token, birthDate) {
  return request('/api/User/BirthDate', {
    method: 'POST',
    token,
    body: { birthDate },
  });
}

export function getDocumentAndJobTypes(token) {
  return request('/api/Dictionary/DocumentAndJobTypes', { token });
}
