import React, { useState, useEffect } from 'react';
import '../../styles/Admin.css';
import AuthService from '../../services/AuthService';
import { useConfiguration } from '../../context/ConfigurationContext';
import Pagination from '../../components/Pagination';

const JurisdictionSetup = () => {
  const [officers, setOfficers] = useState([]);
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize] = useState(10);
  const [totalCount, setTotalCount] = useState(0);

  const [selectedOfficer, setSelectedOfficer] = useState(null);
  const [selectedJurisdiction, setSelectedJurisdiction] = useState("");
  const [showModal, setShowModal] = useState(false);
  const [loading, setLoading] = useState(false);
  const [alert, setAlert] = useState({ show: false, message: '', type: 'danger' });

  const showAlert = (message, type = 'danger') => {
    setAlert({ show: true, message, type });
    setTimeout(() => setAlert({ show: false, message: '', type: 'danger' }), 5000);
  };

  const { configuration } = useConfiguration();
  const jurisdictions = configuration?.jurisdictionMaster || [];

  const fetchJurisdictions = async () => {
    setLoading(true);
    try {
      const res = await AuthService.PostServiceCallTokenWithToken(
        "Admin/DoGetJurisdiction",
        {
          pageNumber: pageNumber,
          pageSize: pageSize
        }
      );

      // Map backend response specifically
      if (res && res.responseCode === 200 && res.data) {
        setOfficers(res.data.gridDetails || []);
        setTotalCount(res.data.totalCount || 0);
      } else {
        setOfficers([]);
        setTotalCount(0);
      }
    } catch (error) {
      console.error("Failed to fetch jurisdictions", error);
      setOfficers([]);
      setTotalCount(0);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchJurisdictions();
  }, [pageNumber]);

  const updateJurisdiction = async () => {
    try {
      if (!selectedOfficer || !selectedJurisdiction) return;

      await AuthService.PostServiceCallTokenWithToken(
        "Admin/DoAssignJurisdiction",
        {
          userIdentifier: selectedOfficer.identifier,
          identifier: selectedJurisdiction
        }
      );

      setShowModal(false);
      fetchJurisdictions(); // refresh table
      showAlert("Jurisdiction updated successfully.", "success");
    } catch (error) {
      console.error("Failed to update jurisdiction", error);
      showAlert("Failed to update jurisdiction.", "danger");
    }
  };

  return (
    <>
      {alert.show && (
        <div className={`alert alert-${alert.type} alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3 shadow-lg`} role="alert" style={{ zIndex: 9999 }}>
          {alert.type === 'success' ? <i className="bi bi-check-circle-fill me-2"></i> : 
           alert.type === 'warning' ? <i className="bi bi-exclamation-triangle-fill me-2"></i> :
           <i className="bi bi-exclamation-circle-fill me-2"></i>}
          {alert.message}
          <button type="button" className="btn-close" onClick={() => setAlert({ ...alert, show: false })} aria-label="Close"></button>
        </div>
      )}
      <div className="fade-in">
        <div className="cms-card">
        <div className="cms-card-header">
          <h5 className="cms-card-title">Officer Deployment Map</h5>
        </div>
        <div className="table-responsive">
          {loading ? (
             <div className="text-center p-4">Loading...</div>
          ) : (
            <>
              <table className="cms-table">
                <thead>
                  <tr>
                    <th>Officer Details</th>
                    <th className="d-none d-md-table-cell">Badge ID</th>
                    <th>Assigned Zone</th>
                    <th className="d-none d-lg-table-cell">Current Status</th>
                    <th className="text-end">Action</th>
                  </tr>
                </thead>
                <tbody>
                  {officers.map(officer => (
                    <tr key={officer.identifier}>
                      <td><span className="fw-bold">{officer.name}</span></td>
                      <td className="d-none d-md-table-cell">
                        <code className="bg-light p-1 rounded text-dark">
                          {officer.identifier ? officer.identifier.substring(0,8) : 'N/A'}
                        </code>
                      </td>
                      <td>{officer.jurisdiction || 'Unassigned'}</td>
                      <td className="d-none d-lg-table-cell">
                        <span className={`cms-status-badge ${officer.jurisdiction ? 'status-active' : 'status-inactive'}`}>
                          {officer.jurisdiction ? 'Assigned' : 'Pending'}
                        </span>
                      </td>
                      <td className="text-end">
                        <button 
                          className="cms-btn cms-btn-primary btn-sm px-4" 
                          onClick={() => {
                            setSelectedOfficer(officer);
                            setSelectedJurisdiction(""); // Reset jurisdiction state hook
                            setShowModal(true);
                          }}
                        >
                          <i className="bi bi-geo-alt"></i> Reassign
                        </button>
                      </td>
                    </tr>
                  ))}
                  {officers.length === 0 && (
                    <tr>
                      <td colSpan="5" className="text-center text-muted py-4">No officers found.</td>
                    </tr>
                  )}
                </tbody>
              </table>

              <Pagination
                currentPage={pageNumber + 1}
                pageSize={pageSize}
                totalCount={totalCount}
                onPageChange={(page) => setPageNumber(page - 1)}
              />
            </>
          )}
        </div>
      </div>
      </div>

      {showModal && (
        <div className="modal fade show d-block" style={{ backgroundColor: 'rgba(10, 31, 68, 0.8)' }}>
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content shadow-lg">
              <div className="modal-header border-0 pb-0 px-4 pt-4">
                <h5 className="cms-card-title">Update Jurisdiction</h5>
                <button type="button" className="btn-close" onClick={() => setShowModal(false)} aria-label="Close"></button>
              </div>
              <div className="modal-body p-4">
                <div className="cms-form">
                  <div className="cms-form-group">
                    <label className="cms-label">Officer Selection</label>
                    <select 
                      className="cms-select"
                      value={selectedOfficer ? selectedOfficer.identifier : ""}
                      onChange={(e) => {
                        const obj = officers.find(o => o.identifier === e.target.value);
                        if (obj) setSelectedOfficer(obj);
                      }}
                    >
                      {officers.map(o => <option key={o.identifier} value={o.identifier}>{o.name}</option>)}
                    </select>
                  </div>
                  <div className="cms-form-group">
                    <label className="cms-label">Target Jurisdiction</label>
                    <select 
                      className="cms-select"
                      value={selectedJurisdiction}
                      onChange={(e) => setSelectedJurisdiction(e.target.value)}
                    >
                      <option value="">-- Select Jurisdiction --</option>
                      {jurisdictions.map(j => (
                        <option key={j.identifier} value={j.identifier}>
                          {j.name}
                        </option>
                      ))}
                    </select>
                  </div>
                </div>
              </div>
              <div className="modal-footer border-0 p-4 pt-0 d-flex gap-2">
                <button className="cms-btn cms-btn-outline flex-grow-1" onClick={() => setShowModal(false)}>Cancel</button>
                <button className="cms-btn cms-btn-primary flex-grow-1" onClick={updateJurisdiction}>
                  Save Changes
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default JurisdictionSetup;
