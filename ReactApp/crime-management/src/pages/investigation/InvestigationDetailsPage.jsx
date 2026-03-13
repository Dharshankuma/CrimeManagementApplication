import React, { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import StatusBadge from '../../components/StatusBadge';
import LoadingSpinner from '../../components/LoadingSpinner';
import { useAuth, UserRole } from '../../context/AuthContext';
import AuthService from '../../services/AuthService';

const InvestigationDetailsPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);
  const [complaint, setComplaint] = useState(null);

  const [newStatus, setNewStatus] = useState('');
  const [closingReason, setClosingReason] = useState('');
  const [comment, setComment] = useState('');
  const [updating, setUpdating] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [alert, setAlert] = useState({ show: false, message: '', type: 'danger' });
  const fileInputRef = useRef(null);

  const showAlert = (message, type = 'danger') => {
    setAlert({ show: true, message, type });
    setTimeout(() => setAlert({ show: false, message: '', type: 'danger' }), 5000);
  };

  const formatDateForInput = (dateStr) => {
    if (!dateStr || typeof dateStr !== 'string') return '';
    const parts = dateStr.split('-');
    if (parts.length === 3 && parts[0].length === 2) {
      // Convert dd-mm-yyyy to yyyy-mm-dd
      return `${parts[2]}-${parts[1]}-${parts[0]}`;
    }
    return dateStr;
  };

  const fetchDetails = async () => {
    setLoading(true);
    setError(false);
    try {
      const response = await AuthService.GetServiceCallWithToken(`CrimeReport/FetchComplaintDetails/${id}`);
      if (response && response.data) {
        const data = response.data;
        if (data.investigation && data.investigation.startDate) {
          data.investigation.startDate = formatDateForInput(data.investigation.startDate);
        }
        setComplaint(data);
      } else {
        setError(true);
      }
    } catch (err) {
      setError(true);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchDetails();
  }, [id]);

  if (loading) return <LoadingSpinner />;
  if (error || !complaint) return <div className="alert alert-danger">Failed to load investigation details</div>;

  const isClosed = complaint.workflow?.currentStatus === 'Closed - No Action' || complaint.workflow?.currentStatus === 'Closed';
  const allowedNext = complaint.workflow?.availableNextStatus || [];
  const showReasonField = newStatus === 'Closed - No Action';

  const handleUpdateStatus = () => {
    if (!newStatus || !complaint.investigation?.startDate || !complaint.investigation?.investigationDescription?.trim()) {
      showAlert("Please provide the Update Status, Start Date, and Field Notes before proceeding.", "warning");
      return;
    }

    if (newStatus === 'Closed - No Action') {
      setShowModal(true);
    } else {
      processStatusUpdate();
    }
  };

  const processStatusUpdate = async () => {
    if (!newStatus || !complaint.investigation?.startDate || !complaint.investigation?.investigationDescription?.trim()) {
      showAlert("Please provide the Update Status, Start Date, and Field Notes before proceeding.", "warning");
      setShowModal(false);
      return;
    }

    setUpdating(true);

    const getFormattedDate = () => {
      const today = new Date();
      return `${today.getDate().toString().padStart(2, '0')}-${(today.getMonth() + 1).toString().padStart(2, '0')}-${today.getFullYear()}`;
    };

    const formatDateForApi = (dateStr) => {
      if (!dateStr) return null;
      const parts = dateStr.split('-');
      if (parts.length === 3 && parts[0].length === 4) {
        return `${parts[2]}-${parts[1]}-${parts[0]}`;
      }
      return dateStr;
    };

    const payload = {
      Identifier: complaint.investigation?.identifier,
      userIdentifier: user?.identifier,
      statusIdentifier: newStatus,
      Priority: complaint.investigation?.priority,
      InvestigationDescription: complaint.investigation?.investigationDescription,
      startDateString: formatDateForApi(complaint.investigation?.startDate),
      endDateString: newStatus === 'Closed - No Action' ? getFormattedDate() : null
    };

    try {
      const response = await AuthService.PostServiceCallTokenWithToken("Investigation/UpdateInvestigation", payload);
      if (response && response.success === false) {
        showAlert("Failed to update investigation: " + (response.message || "Unknown error"), "danger");
      } else {
        showAlert("Investigation updated successfully", "success");
        setShowModal(false);
        setTimeout(() => navigate("/investigations"), 1500);
      }
    } catch (error) {
      showAlert("Failed to update investigation", "danger");
    } finally {
      setUpdating(false);
    }
  };

  const handleDownloadEvidence = async (identifier) => {
    try {
      const response = await AuthService.PostServiceCallTokenWithToken(`EvidenceAttachment/DoDownloadEvidence/${identifier}`, {});
      if (response && response.data) {
        const { fileName, contentType, base64Content } = response.data;

        const byteCharacters = atob(base64Content);
        const byteNumbers = new Array(byteCharacters.length);

        for (let i = 0; i < byteCharacters.length; i++) {
          byteNumbers[i] = byteCharacters.charCodeAt(i);
        }

        const byteArray = new Uint8Array(byteNumbers);
        const blob = new Blob([byteArray], { type: contentType });

        const url = window.URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.href = url;
        link.download = fileName;
        link.click();

        window.URL.revokeObjectURL(url);
      } else {
        showAlert("Failed to download file", "danger");
      }
    } catch (error) {
      showAlert("Failed to download file", "danger");
    }
  };

  const handleFileUpload = async (event) => {
    const files = event.target.files;
    if (!files || files.length === 0) return;

    const allowedExtensions = ['jpg', 'jpeg', 'png', 'pdf'];
    const maxFileSize = 10 * 1024 * 1024; // 10MB

    const formData = new FormData();
    formData.append("complaintIdentifier", complaint.complaintIdentifier);

    for (let i = 0; i < files.length; i++) {
      const file = files[i];
      const fileExt = file.name.split('.').pop().toLowerCase();

      if (!fileExt || !allowedExtensions.includes(fileExt)) {
        showAlert("Only JPG, JPEG, PNG, and PDF files are allowed", "warning");
        if (fileInputRef.current) fileInputRef.current.value = "";
        return;
      }

      if (file.size > maxFileSize) {
        showAlert("File size must be less than or equal to 10 MB", "warning");
        if (fileInputRef.current) fileInputRef.current.value = "";
        return;
      }

      formData.append("files", file);
    }

    try {
      const response = await AuthService.PostServiceCallTokenWithToken("EvidenceAttachment/DoUploadEvidence", formData);
      if (response && response.success === false) {
        showAlert("Failed to upload evidence", "danger");
      } else {
        showAlert("Evidence uploaded successfully", "success");
        fetchDetails();
      }
    } catch (error) {
      showAlert("Failed to upload evidence", "danger");
    }

    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  return (
    <div className="investigation-details">
      {alert.show && (
        <div className={`alert alert-${alert.type} alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3 shadow-lg`} role="alert" style={{ zIndex: 9999 }}>
          {alert.type === 'success' ? <i className="bi bi-check-circle-fill me-2"></i> :
            alert.type === 'warning' ? <i className="bi bi-exclamation-triangle-fill me-2"></i> :
              <i className="bi bi-exclamation-circle-fill me-2"></i>}
          {alert.message}
          <button type="button" className="btn-close" onClick={() => setAlert({ ...alert, show: false })} aria-label="Close"></button>
        </div>
      )}
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <button className="btn btn-link ps-0 text-decoration-none text-muted mb-2" onClick={() => navigate(-1)}>
            <i className="bi bi-arrow-left me-1"></i> Back to List
          </button>
          <h2 className="fw-bold mb-0">Case File: 
            <code className="ms-2 bg-light px-3 py-1 rounded text-primary border" style={{ fontSize: '0.9em' }}>
              {complaint.complaintIdentifier ? complaint.complaintIdentifier.substring(0, 12).toUpperCase() : 'N/A'}
            </code>
          </h2>
        </div>
        <div>
          <StatusBadge status={complaint.workflow?.CurrentStatus} />
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
                  <span className="fw-bold">{complaint.complaintName}</span>
                </div>
                <div className="col-md-6 mb-3">
                  <label className="text-muted small d-block">Crime Type</label>
                  <span className="fw-bold">{complaint.crimeType}</span>
                </div>
                <div className="col-md-6 mb-3">
                  <label className="text-muted small d-block">Jurisdiction</label>
                  <span className="fw-bold">{complaint.jurisdiction}</span>
                </div>
                <div className="col-md-6 mb-3">
                  <label className="text-muted small d-block">Incident Date</label>
                  <span className="fw-bold">{complaint.incidentDate}</span>
                </div>
                <div className="col-12">
                  <label className="text-muted small d-block">Full Description</label>
                  <p className="mb-0 text-secondary">
                    {complaint.crimeDescription}
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
                {/* <div className="col-md-4">
                  <label className="form-label small">Priority Level</label>
                  <select
                    className="form-select form-select-sm"
                    value={complaint.investigation?.priority || "Medium"}
                    disabled={user?.role === UserRole.PUBLIC || user?.role === UserRole.ADMIN}
                    onChange={() => { }}
                  >
                    <option value="Low">Low</option>
                    <option value="Medium">Medium</option>
                    <option value="High">High</option>
                    <option value="Critical">Critical</option>
                  </select>
                </div> */}
                <div className="col-md-4">
                  <label className="form-label small">Assigned Officer</label>
                  <input type="text" className="form-control form-control-sm" value={complaint.investigation?.assignedIo || ""} disabled />
                </div>
                <div className="col-md-4">
                  <label className="form-label small">Start Date</label>
                  <input type="date" className="form-control form-control-sm" value={complaint.investigation?.startDate || ""} disabled={user?.role === UserRole.PUBLIC || user?.role === UserRole.ADMIN || isClosed} onChange={(e) => setComplaint({ ...complaint, investigation: { ...complaint.investigation, startDate: e.target.value } })} />
                </div>
                <div className="col-12">
                  <label className="form-label small">Field Notes</label>
                  <textarea
                    className="form-control"
                    rows={3}
                    placeholder="Enter investigation progress notes..."
                    value={complaint.investigation?.investigationDescription || ""}
                    disabled={user?.role === UserRole.PUBLIC || user?.role === UserRole.ADMIN || isClosed}
                    onChange={(e) => setComplaint({ ...complaint, investigation: { ...complaint.investigation, investigationDescription: e.target.value } })}
                  ></textarea>
                </div>
              </div>
            </div>
          </div>

          {/* C. Evidence Section */}
          <div className="card shadow-sm border-0 mb-4">
            <div className="card-header bg-white py-3 border-0 d-flex justify-content-between align-items-center">
              <h5 className="mb-0 fw-bold"><i className="bi bi-box-seam me-2 text-info"></i>Evidence Repository</h5>
              {user?.role !== UserRole.PUBLIC && !isClosed && (
                <>
                  <input
                    type="file"
                    multiple
                    accept=".jpg,.jpeg,.png,.pdf"
                    ref={fileInputRef}
                    style={{ display: "none" }}
                    disabled={user?.role === UserRole.PUBLIC || user?.role === UserRole.ADMIN || isClosed}
                    onChange={handleFileUpload}
                  />
                  <button
                    className="btn btn-sm btn-outline-primary"
                    onClick={() => fileInputRef.current.click()}
                  >
                    <i className="bi bi-upload me-1"></i> Upload
                  </button>
                </>
              )}
            </div>
            <div className="card-body p-0">
              {complaint.evidenceAttachments && complaint.evidenceAttachments.length > 0 ? (
                <ul className="list-group list-group-flush">
                  {complaint.evidenceAttachments.map((item, index) => (
                    <li
                      key={item.identifier || index}
                      className="list-group-item d-flex justify-content-between align-items-center py-3 px-4"
                    >
                      <div className="d-flex align-items-center">
                        <div className="bg-light p-2 rounded me-3">
                          <i className="bi bi-file-earmark-text text-secondary h4 mb-0"></i>
                        </div>
                        <div>
                          <div className="fw-bold">{item.evidenceName}</div>
                          <div className="small text-muted">{item.createdDate}</div>
                        </div>
                      </div>

                      <button
                        className="btn btn-light btn-sm rounded-circle"
                        onClick={() => handleDownloadEvidence(item.identifier)}
                      >
                        <i className="bi bi-download"></i>
                      </button>
                    </li>
                  ))}
                </ul>
              ) : (
                <div className="text-center py-4 text-muted">
                  <i className="bi bi-folder-x fs-3 d-block mb-2"></i>
                  No evidence uploaded
                </div>
              )}
            </div>

          </div>

          {/* D. Comments Section */}
          {/* <div className="card shadow-sm border-0">
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
                  <button className="btn btn-primary px-4" type="button" onClick={() => setComment('')} disabled={user?.role === UserRole.PUBLIC || user?.role === UserRole.ADMIN || isClosed}>Post</button>
                </div>
              </div>

              <div className="comments-list">
                {complaint.comments?.map((c, index) => (
                  <div key={index} className="d-flex mb-3">
                    <div className="avatar bg-secondary-subtle rounded-circle d-flex align-items-center justify-content-center me-3" style={{ width: '40px', height: '40px', flexShrink: 0 }}>
                      {c.commentBy ? c.commentBy.charAt(0).toUpperCase() : 'U'}
                    </div>
                    <div className="bg-light p-3 rounded-3 w-100">
                      <div className="d-flex justify-content-between align-items-center mb-1">
                        <span className="fw-bold small">{c.commentBy}</span>
                        <span className="text-muted extra-small" style={{ fontSize: '0.75rem' }}>{c.commentDate}</span>
                      </div>
                      <p className="mb-0 small">{c.commentText}</p>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div> */}
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
                <div className="h5 fw-bold mb-0 text-primary">{complaint.workflow?.currentStatus || 'Unknown'}</div>
              </div>

              {(user?.role === UserRole.ADMIN || user?.role === UserRole.OFFICER) && !isClosed ? (
                <div className="mt-4 p-3 border rounded bg-light">
                  <label className="form-label small fw-bold">Update Status</label>
                  <select
                    className="form-select mb-3"
                    value={newStatus}
                    disabled={user?.role === UserRole.PUBLIC || user?.role === UserRole.ADMIN}
                    onChange={(e) => {
                      setNewStatus(e.target.value);
                      // if (e.target.value !== 'Closed - No Action') setClosingReason('');
                    }}
                  >
                    <option value="">Select next status...</option>
                    {allowedNext.map(s => (
                      <option key={s.statusIdentifier} value={s.statusIdentifier}>{s.statusName}</option>
                    ))}
                  </select>

                  {/* {showReasonField && (
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
                  )} */}

                  <button
                    className="btn btn-primary w-100"
                    disabled={updating}
                    onClick={handleUpdateStatus}
                  >
                    {updating ? 'Updating...' : 'Commit Status Change'}
                  </button>
                </div>
              ) : (
                <div className="alert alert-info py-2 small mb-0">
                  {isClosed ? 'This case is finalized and closed.' : 'Case is under investigation'}
                </div>
              )}
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
