import { GoogleLogin } from "@react-oauth/google";
import { useNavigate } from "react-router-dom";
import { useAdminAuth } from "./AdminAuthContext";
import { request } from "../api/http";

export default function AdminLoginPage() {
  const navigate = useNavigate();
  const { loginAdmin } = useAdminAuth();

  const onSuccess = async (cred) => {
    try {
        if (!cred?.credential) {
            alert("Google nie zwrócił tokena (credential).");
            return;
        }
      const data = await request("/api/auth/google-login-admin", {
        method: "POST",
        body: { token: cred.credential },
      });

      loginAdmin({
        token: data.token,
        email: data.email,
        role: data.role,
      });

      navigate("/admin/offers");
    } catch (err) {
        console.error(err);
        alert(err?.message || "Błąd logowania admina");
    }
  };

  return (
    <div className="card" style={{ maxWidth: 520, margin: "40px auto" }}>
      <h2>Logowanie admina</h2>
      <GoogleLogin onSuccess={onSuccess} onError={() => alert("Login failed")} />
    </div>
  );
}
