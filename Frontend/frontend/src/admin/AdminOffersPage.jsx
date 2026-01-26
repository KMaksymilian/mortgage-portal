import { useEffect, useState } from "react";
import { useAdminAuth } from "./AdminAuthContext";
import { request } from "../api/http";


const STATUS_LABELS = {
  0: "Created",
  1: "Pending",
  2: "Approved",
  3: "Rejected",
  4: "ContractSigned",
  5: "Completed",
  6: "Canceled",
};

function formatStatus(status) {
  if (typeof status === "string") return status;
  return STATUS_LABELS[status] ?? String(status);
}

function statusColor(statusText) {
  switch (statusText) {
    case "Pending": return "#fbbf24";
    case "Approved": return "#4CAF50";
    case "Rejected": return "#ff6b6b";
    case "ContractSigned": return "#60a5fa";
    case "Completed": return "#a78bfa";
    case "Canceled": return "#9ca3af";
    default: return "#9ca3af";
  }
}


export default function AdminOffersPage() {
  const { admin, logoutAdmin } = useAdminAuth();
  const [offers, setOffers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [userModal, setUserModal] = useState({
    open: false,
    loading: false,
    data: null,
    error: null,
    expectedEmail: null,
    });

const openUserModal = async (offerId, expectedEmail) => {
  setUserModal({ open: true, loading: true, data: null, error: null, expectedEmail });

  try {
    const u = await request(`/api/admin/offers/${offerId}/user`, {
      token: admin.token,
    });
    setUserModal({ open: true, loading: false, data: u, error: null, expectedEmail });
  } catch (e) {
    setUserModal({
      open: true,
      loading: false,
      data: null,
      error: e?.message || "Błąd pobierania danych usera",
      expectedEmail,
    });
  }
};


const closeUserModal = () =>
    setUserModal({ open: false, loading: false, data: null, error: null, expectedEmail: null });



  const load = async () => {
    setLoading(true);
    try {
      const data = await request("/api/admin/offers", {
        token: admin.token,
      });
      setOffers(Array.isArray(data) ? data : []);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const approve = async (id) => {
    if (!confirm("Approve?")) return;
    await request(`/api/admin/offers/${id}/approve`, {
      method: "POST",
      token: admin.token,
    });
    load();
  };

  const reject = async (id) => {
    const reason = prompt("Powód odrzucenia:");
    if (reason === null) return;
    await request(`/api/admin/offers/${id}/reject`, {
      method: "POST",
      token: admin.token,
      body: { reason },
    });
    load();
  };

  return (
    <div className="card" style={{ maxWidth: 1100, margin: "30px auto" }}>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", gap: 12 }}>
        <h2 style={{ margin: 0 }}>Admin panel — oferty</h2>
        <button className="btn" onClick={logoutAdmin}>Wyloguj</button>
      </div>

      {loading ? (
        <p>Ładowanie...</p>
      ) : offers.length === 0 ? (
        <p>Brak ofert Pending / ContractSigned</p>
      ) : (
        <div style={{ overflowX: "auto", marginTop: 16 }}>
          <table style={{ width: "100%", borderCollapse: "collapse", minWidth: 900 }}>
            <thead>
              <tr style={{ textAlign: "left", opacity: 0.85 }}>
                <th style={{ padding: 12 }}>ID</th>
                <th style={{ padding: 12 }}>Status</th>
                <th style={{ padding: 12 }}>User</th>
                <th style={{ padding: 12 }}>Kwota</th>
                <th style={{ padding: 12 }}>Rata</th>
                <th style={{ padding: 12 }}>Akcje</th>
              </tr>
            </thead>
            <tbody>
            {offers.map((o) => {
                const st = formatStatus(o.status);

                return (
                    <tr key={o.id} style={{ borderTop: "1px solid rgba(255,255,255,0.08)" }}>
                    {/* ID */}
                    <td style={{ padding: 12 }}>{o.id}</td>

                    {/* STATUS */}
                    <td style={{ padding: 12 }}>
                        <span
                        style={{
                            display: "inline-block",
                            padding: "6px 10px",
                            borderRadius: 999,
                            border: `1px solid ${statusColor(st)}`,
                            color: statusColor(st),
                            fontWeight: 800,
                            fontSize: 12,
                            background: "rgba(0,0,0,0.25)",
                        }}
                        >
                        {st}
                        </span>
                    </td>

                    {/* USER */}
                    <td style={{ padding: 12 }}>
                        <button
                        onClick={() => openUserModal(o.id, o.userEmail)}
                        style={{
                            display: "inline-flex",
                            alignItems: "center",
                            gap: 8,
                            padding: "6px 10px",
                            borderRadius: 999,
                            border: "1px solid rgba(96,165,250,0.7)",
                            background: "rgba(96,165,250,0.12)",
                            color: "white",
                            cursor: "pointer",
                            fontWeight: 800,
                        }}
                        >
                        {o.userEmail}
                        </button>
                    </td>

                    <td style={{ padding: 12 }}>
                        {o.requestedAmount} {o.requestedCurrency}
                    </td>

                    <td style={{ padding: 12 }}>
                        {o.monthlyInstallmentAmount} {o.monthlyInstallmentCurrency}
                    </td>

                    <td style={{ padding: 12, display: "flex", gap: 10 }}>
                        <button className="btn" onClick={() => approve(o.id)}>Approve</button>
                        <button
                        className="btn"
                        onClick={() => reject(o.id)}
                        style={{ borderColor: "#ff6b6b", color: "#ff6b6b" }}
                        >
                        Reject
                        </button>
                    </td>
                    </tr>
                );
            })}
            </tbody>
          </table>
        </div>
      )}

      {userModal.open && (
        <div
            onClick={closeUserModal}
            style={{
            position: "fixed",
            inset: 0,
            background: "rgba(0,0,0,0.75)",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            zIndex: 9999,
            padding: 16,
            }}
        >
            <div
            onClick={(e) => e.stopPropagation()}
            className="card"
            style={{
                width: "min(780px, 100%)",
                padding: 22,
                backgroundColor: "#0b1220",
                border: "1px solid rgba(255,255,255,0.12)",
                boxShadow: "0 20px 80px rgba(0,0,0,0.6)",
                borderRadius: 16,
            }}
            >
            <div
                style={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                gap: 12,
                }}
            >
                <h3 style={{ margin: 0 }}>Profil użytkownika</h3>
                <button className="btn" onClick={closeUserModal}>
                Zamknij
                </button>
            </div>

            {userModal.loading ? (
                <p style={{ marginTop: 16 }}>Ładowanie…</p>
            ) : userModal.error ? (
                <p style={{ marginTop: 16, color: "#ff6b6b" }}>{userModal.error}</p>
            ) : (
                (() => {
                const u = userModal.data || {};
                const shownEmail = u.email || u.Email || userModal.expectedEmail || "-";

                const incomeAmount = u?.income?.amount ?? u?.income?.Amount ?? "-";
                const incomeCurrency = u?.income?.currencyCode ?? u?.income?.CurrencyCode ?? "";

                const cardStyle = (accent) => ({
                    padding: 18,
                    borderRadius: 12,
                    border: `1px solid ${accent}`,
                    background: "rgba(255,255,255,0.03)",
                });

                return (
                    <div style={{ marginTop: 16, display: "grid", gap: 14 }}>
                        <div style={cardStyle("rgba(255,255,255,0.18)")}>
                            <h4 style={{ marginTop: 0 }}>Dane Osobowe</h4>
                            <div><b>Imię i Nazwisko:</b> {(u.firstName ?? "-")} {(u.lastName ?? "")}</div>
                            <div><b>Email:</b> {shownEmail}</div>
                            <div><b>Data urodzenia:</b> {u.birthDate ?? "-"}</div>
                            <div><b>Zarobki:</b> {incomeAmount} {incomeCurrency}</div>
                        </div>

                        <div style={cardStyle("rgba(96,165,250,0.5)")}>
                            <h4 style={{ marginTop: 0 }}>Twoja Praca</h4>
                            {u.job ? (
                                <>
                                <div><b>Stanowisko:</b> {u.job.name}</div>
                                <div><b>Opis:</b> {u.job.description}</div>
                                </>
                            ) : (
                                <div>Brak przypisanej pracy.</div>
                            )}

                            <div style={{ marginTop: 10 }}>
                                <b>Start:</b> {u.startDate ? new Date(u.startDate).toLocaleDateString() : "-"}
                            </div>
                            <div>
                                <b>Koniec:</b> {u.endDate ? new Date(u.endDate).toLocaleDateString() : "-"}
                            </div>
                        </div>

                        <div style={cardStyle("rgba(34,197,94,0.5)")}>
                        <h4 style={{ marginTop: 0 }}>Dokument Tożsamości</h4>
                        {u.document ? (
                            <>
                            <div><b>Typ dokumentu:</b> {u.document.name}</div>
                            <div><b>Opis:</b> {u.document.description}</div>
                            </>
                        ) : (
                            <div>Brak przypisanego dokumentu.</div>
                        )}
                        </div>
                    </div>
                );
                })()
            )}
            </div>
        </div>
        )}


    </div>
  );
}
