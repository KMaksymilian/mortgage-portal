import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import './index.css';
import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from './AuthContext';
import { GoogleOAuthProvider } from '@react-oauth/google';
import { AdminAuthProvider } from './admin/AdminAuthContext';

const GOOGLE_CLIENT_ID = "786471780812-iq2jnhgem44amino0dctajvbjp2bi052.apps.googleusercontent.com";

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <GoogleOAuthProvider clientId={GOOGLE_CLIENT_ID}>
      <BrowserRouter>
        <AdminAuthProvider>
          <AuthProvider>
            <App />
          </AuthProvider>
        </AdminAuthProvider>
      </BrowserRouter>
    </GoogleOAuthProvider>
  </React.StrictMode>
);
