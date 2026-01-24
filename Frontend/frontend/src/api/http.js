export class ApiError extends Error {
  constructor(message, { status, url, details } = {}) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.url = url;
    this.details = details;
  }
}

async function readErrorBody(res) {
  const contentType = res.headers.get('content-type') || '';
  try {
    if (contentType.includes('application/json')) return await res.json();
    return await res.text();
  } catch {
    return null;
  }
}

export async function request(url, {
  method = 'GET',
  token,
  body,
  headers,
  responseType = 'json', // 'json' | 'text' | 'blob'
} = {}) {
  const finalHeaders = {
    ...(headers || {}),
  };

  if (token) finalHeaders.Authorization = `Bearer ${token}`;

  const hasBody = body !== undefined && body !== null;

  if (hasBody && !finalHeaders['Content-Type']) {
    finalHeaders['Content-Type'] = 'application/json';
  }

  const res = await fetch(url, {
    method,
    headers: finalHeaders,
    body: hasBody ? JSON.stringify(body) : undefined,
  });

  if (!res.ok) {
    const details = await readErrorBody(res);
    const msg =
      (typeof details === 'string' && details.trim()) ||
      (details?.message) ||
      `Request failed: ${res.status}`;

    throw new ApiError(msg, { status: res.status, url, details });
  }

  if (responseType === 'blob') return await res.blob();
  if (responseType === 'text') return await res.text();
  if (res.status === 204) return null;

  return await res.json();
}
