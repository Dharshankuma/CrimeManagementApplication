import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import StatusBadge from '../components/StatusBadge';
import { COMPLAINTS } from '../services/mockData';

const ComplaintListPage = () => {
  const navigate = useNavigate();
  const [searchTerm, setSearchTerm] = useState('');

  const filteredComplaints = COMPLAINTS.filter(c => 
    c.name.toLowerCase().includes(searchTerm.toLowerCase()) || 
    c.id.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="complaint-list">
      <div className="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 gap-3">
        <div>
          <h2 className="fw-bold mb-1">Complaints Management</h2>
          <p className="text-muted mb-0">Browse and manage all incoming crime reports.</p>
        </div>
        <div className="d-flex gap-2">
          <div className="input-group" style={{ maxWidth: '300px' }}>
            <span className="input-group-text bg-white border-end-0"><i className="bi bi-search"></i></span>
            <input 
              type="text" 
              className="form-control border-start-0 ps-0" 
              placeholder="Search by name or ID..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
          <button className="btn btn-primary"><i className="bi bi-plus-lg me-1"></i> New</button>
        </div>
      </div>

      <div className="card shadow-sm border-0">
        <div className="card-body p-0">
          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead className="table-light">
                <tr>
                  <th className="ps-4 py-3">Complaint Name</th>
                  <th className="py-3">Jurisdiction</th>
                  <th className="py-3">Crime Type</th>
                  <th className="py-3">Status</th>
                  <th className="py-3">Date</th>
                  <th className="pe-4 py-3 text-end">Action</th>
                </tr>
              </thead>
              <tbody>
                {filteredComplaints.map((complaint) => (
                  <tr key={complaint.id}>
                    <td className="ps-4">
                      <div className="fw-bold">{complaint.name}</div>
                      <div className="text-muted small">{complaint.id}</div>
                    </td>
                    <td>{complaint.jurisdiction}</td>
                    <td>{complaint.type}</td>
                    <td><StatusBadge status={complaint.status} /></td>
                    <td>{complaint.date}</td>
                    <td className="pe-4 text-end">
                      <button 
                        className="btn btn-sm btn-outline-primary"
                        onClick={() => navigate(`/investigations/${complaint.id}`)}
                      >
                        View Details
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
        <div className="card-footer bg-white border-0 py-3">
          <nav>
            <ul className="pagination pagination-sm justify-content-center mb-0">
              <li className="page-item disabled"><span className="page-link">Previous</span></li>
              <li className="page-item active"><span className="page-link">1</span></li>
              <li className="page-item"><span className="page-link">2</span></li>
              <li className="page-item"><span className="page-link">3</span></li>
              <li className="page-item"><span className="page-link">Next</span></li>
            </ul>
          </nav>
        </div>
      </div>
    </div>
  );
};

export default ComplaintListPage;
