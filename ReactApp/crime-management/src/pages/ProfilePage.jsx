
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import '../styles/Admin.css';

const ProfilePage = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    fullName: (user?.firstname || '') + " " + (user?.lastname || ''),
    email: user?.emailId || '',
    mobile: '',
    aadhar: '',
    address: ''
  });

  const [showSuccess, setShowSuccess] = useState(false);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleUpdate = (e) => {
    e.preventDefault();
    setShowSuccess(true);
    setTimeout(() => setShowSuccess(false), 3000);
  };

  const handleResetToken = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="admin-container">
      <div className="container">
        <div className="row justify-content-center">
          <div className="col-lg-8">
            <div className="mb-4">
              <h1 className="cms-title">Officer Profile</h1>
              <p className="cms-subtitle">Manage your personal and system identification information</p>
            </div>

            {showSuccess && (
              <div className="alert fade-in mb-4 d-flex align-items-center" style={{ backgroundColor: '#D1FAE5', color: '#065F46', border: 'none', borderRadius: '12px' }}>
                <i className="bi bi-check-circle-fill me-2 h5 mb-0"></i>
                <div>Your profile details have been successfully updated in the system records.</div>
              </div>
            )}

            <div className="cms-card shadow-lg">
              <div className="cms-card-header d-flex align-items-center gap-4 bg-light">
                <div className="bg-navy rounded-circle d-flex align-items-center justify-content-center text-white" style={{ width: '80px', height: '80px', fontSize: '2.5rem', backgroundColor: '#0A1F44' }}>
                  {formData.fullName.charAt(0)}
                </div>
                <div>
                  <h3 className="cms-card-title mb-1">{formData.fullName}</h3>
                  <div className="badge bg-navy text-white px-3 py-2" style={{ backgroundColor: '#1F3A5F' }}>{user?.role}</div>
                </div>
              </div>
              <div className="card-body p-4">
                <form onSubmit={handleUpdate}>
                  <div className="row g-4">
                    <div className="col-md-6">
                      <div className="cms-form-group">
                        <label className="cms-label">Full Name</label>
                        <input
                          type="text"
                          name="fullName"
                          className="cms-input"
                          value={formData.fullName}
                          onChange={handleInputChange}
                        />
                      </div>
                    </div>
                    <div className="col-md-6">
                      <div className="cms-form-group">
                        <label className="cms-label">Email Address</label>
                        <input
                          type="email"
                          name="email"
                          className="cms-input"
                          value={formData.email}
                          onChange={handleInputChange}
                        />
                      </div>
                    </div>
                    <div className="col-md-6">
                      <div className="cms-form-group">
                        <label className="cms-label">Mobile Number</label>
                        <input
                          type="text"
                          name="mobile"
                          className="cms-input"
                          value={formData.mobile}
                          onChange={handleInputChange}
                        />
                      </div>
                    </div>
                    <div className="col-md-6">
                      <div className="cms-form-group">
                        <label className="cms-label">Aadhar Number (Linked)</label>
                        <input
                          type="text"
                          name="aadhar"
                          className="cms-input"
                          value={formData.aadhar}
                          disabled
                          title="Contact central admin to change identity verification"
                        />
                        <div className="extra-small text-muted mt-1">Verified via UIDAI</div>
                      </div>
                    </div>
                    <div className="col-12">
                      <div className="cms-form-group">
                        <label className="cms-label">Official Address</label>
                        <textarea
                          name="address"
                          className="cms-textarea"
                          rows="3"
                          value={formData.address}
                          onChange={handleInputChange}
                        ></textarea>
                      </div>
                    </div>
                  </div>

                  <div className="mt-4 pt-3 border-top d-flex justify-content-between align-items-center">
                    <div className="text-muted small">
                      <i className="bi bi-shield-lock me-1"></i>
                      Last updated: Today, 10:45 AM
                    </div>
                    <button type="submit" className="cms-btn cms-btn-primary px-5">
                      Update Profile Information
                    </button>
                  </div>
                </form>
              </div>
            </div>

            <div className="cms-card border-danger mt-4" style={{ borderColor: 'rgba(239, 68, 68, 0.2) !important' }}>
              <div className="card-body p-4">
                <h5 className="cms-card-title text-danger mb-3">System Access</h5>
                <p className="small text-muted mb-3">If you suspect your credentials have been compromised, please reset your authentication key immediately.</p>
                <button className="cms-btn cms-btn-danger-outline" onClick={handleResetToken}>
                  <i className="bi bi-key"></i> Reset Authentication Token
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProfilePage;
