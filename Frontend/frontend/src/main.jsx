import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import './index.css';
import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from './AuthContext';
import { GoogleOAuthProvider } from '@react-oauth/google';

// Twój Client ID z Google Cloud Console
const GOOGLE_CLIENT_ID = "786471780812-iq2jnhgem44amino0dctajvbjp2bi052.apps.googleusercontent.com"; 

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    {/* 1. Dostawca Google OAuth */}
    <GoogleOAuthProvider clientId={GOOGLE_CLIENT_ID}>
      {/* 2. Nasz dostawca Autentykacji */}
      <AuthProvider>
        {/* 3. Router (do obsługi stron) */}
        <BrowserRouter>
          <App />
        </BrowserRouter>
      </AuthProvider>
    </GoogleOAuthProvider>
  </React.StrictMode>
);