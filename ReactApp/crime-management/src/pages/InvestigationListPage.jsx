import React from 'react';
import { useNavigate } from 'react-router-dom';
import StatusBadge from '../components/StatusBadge';
import { COMPLAINTS } from '../services/mockData';

const InvestigationListPage = () => {
  const navigate = useNavigate();
  // Filter for only investigation cases for this demo page
  const investigations = COMPLAINTS.filter(c => c.status === 'Investigation');

  return (
    <div className="investigation-list">
      <div className="mb-4">
        <h2 className="fw-bold mb-1">Active Investigations</h2>
        <p className="text-muted">Ongoing cases currently assigned to field officers.</p>
      </div>

      <div className="card shadow-sm border-0 overflow-hidden">
        <div className="table-responsive">
          <table className="table table-hover align-middle mb-0">
            <thead className="bg-light">
              <tr>
                <th className="ps-4 py-3">Investigation ID</th>
                <th>Priority</th>
                <th>Assigned Officer</th>
                <th>Progress</th>
                <th>Last Update</th>
                <th className="pe-4 text-end">Action</th>
              </tr>
            </thead>
            <tbody>
              {investigations.map((inv, idx) => (
                <tr key={inv.id}>
                  <td className="ps-4">
                    <div className="fw-bold">{inv.id}</div>
                    <div className="small text-muted">{inv.name}</div>
                  </td>
                  <td>
                    <span className={`badge ${idx % 2 === 0 ? 'bg-danger-subtle text-danger' : 'bg-warning-subtle text-warning'}`}>
                      {idx % 2 === 0 ? 'HIGH' : 'MEDIUM'}
                    </span>
                  </td>
                  <td>Off. Sarah Jenkins</td>
                  <td style={{ width: '150px' }}>
                    <div className="progress" style={{ height: '6px' }}>
                      <div className="progress-bar" style={{ width: `${60 + (idx * 15)}%` }}></div>
                    </div>
                  </td>
                  <td>Oct 26, 2023</td>
                  <td className="pe-4 text-end">
                    <button 
                      className="btn btn-sm btn-primary"
                      onClick={() => navigate(`/investigations/${inv.id}`)}
                    >
                      Update Case
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

export default InvestigationListPage;
