import { useEffect, useState } from 'react';
import { useAuth } from './AuthContext';
import { getPendingOffers, approveOffer, rejectOffer } from './api/admin';

export default function AdminOffersPage() {
  const { user } = useAuth();
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);

  const load = async () => {
    setLoading(true);
    try {
      const data = await getPendingOffers(user.token);
      setItems(Array.isArray(data) ? data : []);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const onApprove = async (id) => {
    await approveOffer(user.token, id);
    await load();
  };

  const onReject = async (id) => {
    const reason = prompt('Powód odrzucenia? (opcjonalnie)') ?? '';
    await rejectOffer(user.token, id, reason);
    await load();
  };

  return (
    <div className="card" style={{ margin: '28px auto' }}>
      <h2>Oferty oczekujące (Pending)</h2>

      {loading ? <p>Ładowanie…</p> : null}

      {!loading && items.length === 0 ? (
        <p style={{ color: 'var(--muted)' }}>Brak ofert do moderacji.</p>
      ) : (
        <div style={{ overflowX: 'auto' }}>
          <table className="table" style={{ minWidth: 820 }}>
            <thead>
              <tr>
                <th>ID</th>
                <th>Kwota</th>
                <th>Rata</th>
                <th>Status</th>
                <th style={{ textAlign: 'right' }}>Akcje</th>
              </tr>
            </thead>
            <tbody>
              {items.map(o => (
                <tr key={o.id}>
                  <td>{o.id}</td>
                  <td>{o.loanAmount} {o.currencycode}</td>
                  <td>{o.monthlyInstallment} {o.currencycode}</td>
                  <td>{o.status}</td>
                  <td style={{ textAlign: 'right' }}>
                    <button className="btn" onClick={() => onApprove(o.id)}>Approve</button>{' '}
                    <button className="btn" onClick={() => onReject(o.id)}>Reject</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
