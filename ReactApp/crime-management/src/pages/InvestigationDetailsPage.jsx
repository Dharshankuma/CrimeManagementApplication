import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import StatusBadge from '../components/StatusBadge';
import Timeline from '../components/Timeline';
import LoadingSpinner from '../components/LoadingSpinner';
import { useAuth, UserRole } from '../context/AuthContext';
import { 
  COMPLAINTS, 
  STATUS_WORKFLOW, 
  MOCK_EVIDENCE, 
  MOCK_COMMENTS, 
  MOCK_TIMELINE 
} from '../services/mockData';

const InvestigationDetailsPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();
  const [loading, setLoading] = useState(true);
  const [complaint, setComplaint] = useState(null);
  const [newStatus, setNewStatus] = useState('');
  const [closingReason, setClosingReason] = useState('');
  const [comment, setComment] = useState('');
  const [updating, setUpdating] = useState(false);
  const [showModal, setShowModal] = useState(false);

  useEffect(() => {
    // Simulate API call
    setTimeout(() => {
      const found = COMPLAINTS.find(c => c.id === id);
      if (found) {
        setComplaint(found);
      }
      setLoading(false);
    }, 800);
  }, [id]);

  if (loading) return <LoadingSpinner />;
  if (!complaint) return <div className="alert alert-danger">Case not found.</div>;

  const isClosed = complaint.status === 'Closed' || complaint.status === 'Closed - No Action';
  const allowedNext = STATUS_WORKFLOW[complaint.status] || [];
  const showReasonField = newStatus === 'Closed - No Action';
  
  const handleUpdateStatus = () => {
    if (newStatus === 'Closed - No Action' && !closingReason.trim()) {
      alert('Please provide a reason for closing the case.');
      return;
    }
    
    if (newStatus === 'Closed - No Action') {
      setShowModal(true);
    } else {
      processStatusUpdate();
    }
  };

  const processStatusUpdate = () => {
    setUpdating(true);
    setTimeout(() => {
      setComplaint({ ...complaint, status: newStatus });
      setUpdating(false);
      setNewStatus('');
      setClosingReason('');
      setShowModal(false);
      alert('Case status updated successfully.');
    }, 1000);
  };

  return (
    <div className="investigation-details">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <button className="btn btn-link ps-0 text-decoration-none text-muted mb-2" onClick={() => navigate(-1)}>
            <i className="bi bi-arrow-left me-1"></i> Back to List
          </button>
          <h2 className="fw-bold mb-0">Case File: {complaint.id}</h2>
        </div>
        <div>
          <StatusBadge status={complaint.status} />
        </div>
      </div>

      <div className="row g-4">
        {/* Left Column: Case Information */}
        <div className="col-lg-8">
          
          {/* A. Complaint Details Card */}
          <div className="card shadow-sm border-0 mb-4">
            <div className="card-header bg-white py-3 border-0">
              <h5 className="mb-0 fw-bold"><i className="bi bi-info-circle me-2 text-primary"></i>Complaint Overview</h5>
            </div>
            <div className="card-body">
              <div className="row">
                <div className="col-md-6 mb-3">
                  <label className="text-muted small d-block">Complaint Name</label>
                  <span className="fw-bold">{complaint.name}</span>
                </div>
                <div className="col-md-6 mb-3">
                  <label className="text-muted small d-block">Crime Type</label>
                  <span className="fw-bold">{complaint.type}</span>
                </div>
                <div className="col-md-6 mb-3">
                  <label className="text-muted small d-block">Jurisdiction</label>
                  <span className="fw-bold">{complaint.jurisdiction}</span>
                </div>
                <div className="col-md-6 mb-3">
                  <label className="text-muted small d-block">Incident Date</label>
                  <span className="fw-bold">{complaint.date}</span>
                </div>
                <div className="col-12">
                  <label className="text-muted small d-block">Full Description</label>
                  <p className="mb-0 text-secondary">
                    Detailed report filed by complainant regarding {complaint.name.toLowerCase()}. 
                    Initial assessment indicates potential involvement of local suspects. 
                    Immediate response was deployed to secure the scene.
                  </p>
                </div>
              </div>
            </div>
          </div>

          {/* B. Investigation Details Card */}
          <div className="card shadow-sm border-0 mb-4">
            <div className="card-header bg-white py-3 border-0">
              <h5 className="mb-0 fw-bold"><i className="bi bi-search me-2 text-warning"></i>Investigation Setup</h5>
            </div>
            <div className="card-body">
              <div className="row g-3">
                <div className="col-md-4">
                  <label className="form-label small">Priority Level</label>
                  <select className="form-select form-select-sm" defaultValue="High" disabled={user?.role === UserRole.PUBLIC}>
                    <option>Low</option>
                    <option>Medium</option>
                    <option>High</option>
                    <option>Critical</option>
                  </select>
                </div>
                <div className="col-md-4">
                  <label className="form-label small">Assigned Officer</label>
                  <input type="text" className="form-control form-control-sm" defaultValue="Off. Sarah Jenkins" disabled />
                </div>
                <div className="col-md-4">
                  <label className="form-label small">Start Date</label>
                  <input type="date" className="form-control form-control-sm" defaultValue="2023-10-25" disabled={user?.role === UserRole.PUBLIC} />
                </div>
                <div className="col-12">
                  <label className="form-label small">Field Notes</label>
                  <textarea 
                    className="form-control" 
                    rows={3} 
                    placeholder="Enter investigation progress notes..."
                    disabled={user?.role === UserRole.PUBLIC}
                  ></textarea>
                </div>
              </div>
            </div>
          </div>

          {/* C. Evidence Section */}
          <div className="card shadow-sm border-0 mb-4">
            <div className="card-header bg-white py-3 border-0 d-flex justify-content-between align-items-center">
              <h5 className="mb-0 fw-bold"><i className="bi bi-box-seam me-2 text-info"></i>Evidence Repository</h5>
              {user?.role !== UserRole.PUBLIC && (
                <button className="btn btn-sm btn-outline-primary"><i className="bi bi-upload me-1"></i> Upload</button>
              )}
            </div>
            <div className="card-body p-0">
              <ul className="list-group list-group-flush">
                {MOCK_EVIDENCE.map(item => (
                  <li key={item.id} className="list-group-item d-flex justify-content-between align-items-center py-3 px-4">
                    <div className="d-flex align-items-center">
                      <div className="bg-light p-2 rounded me-3">
                        <i className={`bi bi-file-earmark-${item.type.toLowerCase() === 'image' ? 'image' : 'text'} text-secondary h4 mb-0`}></i>
                      </div>
                      <div>
                        <div className="fw-bold">{item.name}</div>
                        <div className="small text-muted">{item.size} • {item.date}</div>
                      </div>
                    </div>
                    <button className="btn btn-light btn-sm rounded-circle"><i className="bi bi-download"></i></button>
                  </li>
                ))}
              </ul>
            </div>
          </div>

          {/* D. Comments Section */}
          <div className="card shadow-sm border-0">
            <div className="card-header bg-white py-3 border-0">
              <h5 className="mb-0 fw-bold"><i className="bi bi-chat-dots me-2 text-secondary"></i>Case Comments</h5>
            </div>
            <div className="card-body">
              <div className="mb-4">
                <div className="input-group">
                  <textarea 
                    className="form-control" 
                    placeholder="Add a comment or update..." 
                    value={comment}
                    onChange={(e) => setComment(e.target.value)}
                    rows={1}
                  ></textarea>
                  <button className="btn btn-primary px-4" type="button" onClick={() => setComment('')}>Post</button>
                </div>
              </div>
              
              <div className="comments-list">
                {MOCK_COMMENTS.map(c => (
                  <div key={c.id} className="d-flex mb-3">
                    <div className="avatar bg-secondary-subtle rounded-circle d-flex align-items-center justify-content-center me-3" style={{ width: '40px', height: '40px', flexShrink: 0 }}>
                      {c.user.charAt(0)}
                    </div>
                    <div className="bg-light p-3 rounded-3 w-100">
                      <div className="d-flex justify-content-between align-items-center mb-1">
                        <span className="fw-bold small">{c.user}</span>
                        <span className="text-muted extra-small" style={{ fontSize: '0.75rem' }}>{c.date}</span>
                      </div>
                      <p className="mb-0 small">{c.text}</p>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>

        {/* Right Column: Workflow & History */}
        <div className="col-lg-4">
          
          {/* E. Status Section */}
          <div className="card shadow-sm border-0 mb-4 sticky-top" style={{ top: '80px', zIndex: 100 }}>
            <div className="card-header bg-white py-3 border-0">
              <h5 className="mb-0 fw-bold"><i className="bi bi-arrow-repeat me-2 text-success"></i>Workflow Control</h5>
            </div>
            <div className="card-body">
              <div className="mb-3">
                <label className="text-muted small">Current State</label>
                <div className="h5 fw-bold mb-0 text-primary">{complaint.status}</div>
              </div>
              
              {(user?.role === UserRole.ADMIN || user?.role === UserRole.OFFICER) && !isClosed ? (
                <div className="mt-4 p-3 border rounded bg-light">
                  <label className="form-label small fw-bold">Update Status</label>
                  <select 
                    className="form-select mb-3" 
                    value={newStatus} 
                    onChange={(e) => {
                      setNewStatus(e.target.value);
                      if (e.target.value !== 'Closed - No Action') setClosingReason('');
                    }}
                  >
                    <option value="">Select next status...</option>
                    {allowedNext.map(s => <option key={s} value={s}>{s}</option>)}
                  </select>

                  {showReasonField && (
                    <div className="mb-3">
                      <label className="form-label small text-danger fw-bold">Reason for Closure *</label>
                      <textarea 
                        className="form-control" 
                        rows={2} 
                        placeholder="Mandatory explanation..."
                        value={closingReason}
                        onChange={(e) => setClosingReason(e.target.value)}
                        required
                      ></textarea>
                    </div>
                  )}

                  <button 
                    className="btn btn-primary w-100" 
                    disabled={!newStatus || updating}
                    onClick={handleUpdateStatus}
                  >
                    {updating ? 'Updating...' : 'Commit Status Change'}
                  </button>
                </div>
              ) : (
                <div className="alert alert-info py-2 small mb-0">
                  {isClosed ? 'This case is finalized and closed.' : 'Awaiting permissions for state change.'}
                </div>
              )}
            </div>
          </div>

          {/* F. Stage History Timeline */}
          <div className="card shadow-sm border-0">
            <div className="card-header bg-white py-3 border-0">
              <h5 className="mb-0 fw-bold"><i className="bi bi-clock-history me-2 text-muted"></i>Audit Trail</h5>
            </div>
            <div className="card-body">
              <Timeline items={MOCK_TIMELINE} />
            </div>
          </div>
        </div>
      </div>

      {/* Confirmation Modal for False Case */}
      {showModal && (
        <>
          <div className="modal fade show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
            <div className="modal-dialog modal-dialog-centered">
              <div className="modal-content border-0 shadow">
                <div className="modal-header bg-danger text-white border-0">
                  <h5 className="modal-title">Confirm Case Closure</h5>
                  <button type="button" className="btn-close btn-close-white" onClick={() => setShowModal(false)}></button>
                </div>
                <div className="modal-body p-4">
                  <p className="mb-3">You are about to close this case as <strong>"Closed - No Action"</strong>.</p>
                  <p className="text-muted small">Reason provided: <span className="text-dark italic">"{closingReason}"</span></p>
                  <div className="alert alert-warning py-2 small mb-0">
                    <i className="bi bi-exclamation-triangle me-2"></i> This action is permanent and will notify the legal department.
                  </div>
                </div>
                <div className="modal-footer border-0">
                  <button type="button" className="btn btn-light" onClick={() => setShowModal(false)}>Cancel</button>
                  <button type="button" className="btn btn-danger px-4" onClick={processStatusUpdate}>Confirm Closure</button>
                </div>
              </div>
            </div>
          </div>
        </>
      )}
    </div>
  );
};

export default InvestigationDetailsPage;
