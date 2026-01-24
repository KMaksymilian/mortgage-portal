import { request } from './http';

export function googleLogin(googleToken) {
  return request('/api/auth/google-login', {
    method: 'POST',
    body: { token: googleToken },
  });
}
