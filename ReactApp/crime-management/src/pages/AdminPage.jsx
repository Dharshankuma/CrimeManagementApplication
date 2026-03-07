
import React, { useState } from 'react';
import '../styles/Admin.css';

// Mock Data for Admin Modules
const MOCK_MASTERS = {
  crimeTypes: [
    { id: 1, name: 'Cyber Crime', description: 'Crimes involving computers or networks', status: 'Active' },
    { id: 2, name: 'Theft', description: 'Unlawful taking of property', status: 'Active' },
    { id: 3, name: 'Assault', description: 'Physical attack on someone', status: 'Active' },
    { id: 4, name: 'Vandalism', description: 'Destruction of property', status: 'Inactive' },
  ],
  jurisdictions: [
    { id: 1, name: 'Zone A - Downtown', status: 'Active' },
    { id: 2, name: 'Zone B - Suburban', status: 'Active' },
    { id: 3, name: 'Zone C - Coastal', status: 'Active' },
  ]
};

const MOCK_USERS = [
  { id: 1, name: 'Alice Johnson', email: 'alice@example.com', mobile: '9876543210', role: 'Public User', status: 'Active' },
  { id: 2, name: 'Robert Smith', email: 'r.smith@police.gov', mobile: '9123456789', role: 'Officer', status: 'Active' },
  { id: 3, name: 'John Admin', email: 'admin@police.gov', mobile: '9000000000', role: 'Admin', status: 'Active' },
  { id: 4, name: 'Inactive User', email: 'inactive@test.com', mobile: '8888888888', role: 'Public User', status: 'Inactive' },
];

const MOCK_OFFICERS = [
  { id: 1, name: 'Sgt. Miller', badge: 'B-102', jurisdiction: 'Zone A - Downtown', status: 'On Duty' },
  { id: 2, name: 'Det. Holmes', badge: 'B-105', jurisdiction: 'Zone C - Coastal', status: 'On Leave' },
  { id: 3, name: 'Off. Carter', badge: 'B-110', jurisdiction: 'Unassigned', status: 'On Duty' },
];

const AdminPage = () => {
  const [activeSection, setActiveSection] = useState('masters');
  const [activeMasterTab, setActiveMasterTab] = useState('crimeTypes');
  const [searchTerm, setSearchTerm] = useState('');
  
  const [showModal, setShowModal] = useState(false);
  const [modalType, setModalType] = useState('');

  const renderMasters = () => (
    <div className="fade-in">
      <div className="cms-card">
        <div className="cms-card-header">
          <div className="d-flex flex-column flex-lg-row justify-content-between align-items-start align-items-lg-center gap-4">
            <div className="cms-inner-nav-tabs">
              <button 
                className={`cms-inner-tab ${activeMasterTab === 'crimeTypes' ? 'active' : ''}`} 
                onClick={() => setActiveMasterTab('crimeTypes')}
              >
                Crime Types
              </button>
              <button 
                className={`cms-inner-tab ${activeMasterTab === 'jurisdictions' ? 'active' : ''}`} 
                onClick={() => setActiveMasterTab('jurisdictions')}
              >
                Jurisdictions
              </button>
              <button 
                className={`cms-inner-tab ${activeMasterTab === 'status' ? 'active' : ''}`} 
                onClick={() => setActiveMasterTab('status')}
              >
                Status Types
              </button>
              <button 
                className={`cms-inner-tab ${activeMasterTab === 'priorities' ? 'active' : ''}`} 
                onClick={() => setActiveMasterTab('priorities')}
              >
                Priorities
              </button>
            </div>
            <button className="cms-btn cms-btn-primary w-100 w-lg-auto" onClick={() => { setModalType('add'); setShowModal(true); }}>
              <i className="bi bi-plus-circle"></i> Add New Master
            </button>
          </div>
        </div>
        <div className="table-responsive">
          <table className="cms-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                {activeMasterTab === 'crimeTypes' && <th className="d-none d-md-table-cell">Description</th>}
                <th>Status</th>
                <th className="text-end">Actions</th>
              </tr>
            </thead>
            <tbody>
              {MOCK_MASTERS[activeMasterTab === 'crimeTypes' ? 'crimeTypes' : 'jurisdictions']?.map(item => (
                <tr key={item.id}>
                  <td>{item.id}</td>
                  <td className="fw-bold">{item.name}</td>
                  {activeMasterTab === 'crimeTypes' && <td className="text-muted d-none d-md-table-cell">{item.description}</td>}
                  <td>
                    <span className={`cms-status-badge ${item.status === 'Active' ? 'status-active' : 'status-inactive'}`}>
                      {item.status}
                    </span>
                  </td>
                  <td className="text-end">
                    <div className="d-flex gap-2 justify-content-end">
                      <button className="cms-btn cms-btn-outline btn-sm"><i className="bi bi-pencil"></i></button>
                      <button className="cms-btn cms-btn-danger-outline btn-sm"><i className="bi bi-trash"></i></button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );

  const renderUsers = () => (
    <div className="fade-in">
      <div className="cms-card">
        <div className="cms-card-header border-0 pb-0">
          <div className="row g-3">
            <div className="col-12 col-lg-6">
              <div className="cms-form-group mb-0">
                <input 
                  type="text" 
                  className="cms-input" 
                  placeholder="Search by name, email or mobile..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                />
              </div>
            </div>
            <div className="col-12 col-md-6 col-lg-3">
              <select className="cms-select">
                <option value="">Filter by Role</option>
                <option value="Admin">Admin</option>
                <option value="Officer">Officer</option>
                <option value="Public">Public User</option>
              </select>
            </div>
            <div className="col-12 col-md-6 col-lg-3 d-flex justify-content-end">
              <button className="cms-btn cms-btn-primary w-100 justify-content-center">
                <i className="bi bi-search"></i> Apply
              </button>
            </div>
          </div>
        </div>
        <div className="table-responsive mt-3">
          <table className="cms-table">
            <thead>
              <tr>
                <th>User Identity</th>
                <th className="d-none d-md-table-cell">Contact Info</th>
                <th>System Role</th>
                <th>Status</th>
                <th className="text-end">Actions</th>
              </tr>
            </thead>
            <tbody>
              {MOCK_USERS.filter(u => u.name.toLowerCase().includes(searchTerm.toLowerCase())).map(user => (
                <tr key={user.id}>
                  <td>
                    <div className="fw-bold">{user.name}</div>
                    <div className="extra-small text-muted">USR-{user.id}</div>
                  </td>
                  <td className="d-none d-md-table-cell">
                    <div className="small"><i className="bi bi-envelope me-1"></i> {user.email}</div>
                    <div className="small"><i className="bi bi-phone me-1"></i> {user.mobile}</div>
                  </td>
                  <td>
                    <span className="badge border text-dark fw-bold px-2 py-1" style={{ backgroundColor: '#F3F4F6' }}>{user.role}</span>
                  </td>
                  <td>
                    <label className="cms-switch">
                      <input type="checkbox" checked={user.status === 'Active'} readOnly />
                      <span className="slider"></span>
                    </label>
                  </td>
                  <td>
                    <div className="d-flex gap-2 justify-content-end">
                      <button className="cms-btn cms-btn-outline btn-sm"><i className="bi bi-eye"></i></button>
                      <button className="cms-btn cms-btn-outline btn-sm" onClick={() => { setModalType('edit-user'); setShowModal(true); }}>
                        <i className="bi bi-pencil-square"></i>
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );

  const renderJurisdictionAssignment = () => (
    <div className="fade-in">
      <div className="cms-card">
        <div className="cms-card-header">
          <h5 className="cms-card-title">Officer Deployment Map</h5>
        </div>
        <div className="table-responsive">
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
              {MOCK_OFFICERS.map(off => (
                <tr key={off.id}>
                  <td><span className="fw-bold">{off.name}</span></td>
                  <td className="d-none d-md-table-cell"><code className="bg-light p-1 rounded text-dark">{off.badge}</code></td>
                  <td>{off.jurisdiction}</td>
                  <td className="d-none d-lg-table-cell">
                    <span className={`cms-status-badge ${off.status === 'On Duty' ? 'status-active' : 'status-inactive'}`}>
                      {off.status}
                    </span>
                  </td>
                  <td className="text-end">
                    <button className="cms-btn cms-btn-primary btn-sm px-4" onClick={() => { setModalType('assign'); setShowModal(true); }}>
                      <i className="bi bi-geo-alt"></i> Reassign
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

  return (
    <div className="admin-container">
      <div className="container">
        <div className="row mb-5">
          <div className="col-12">
            <h1 className="cms-title">Admin Control Panel</h1>
            <p className="cms-subtitle">Police System Configuration and Personnel Oversight</p>
          </div>
        </div>

        {/* PRIMARY SECTION NAVIGATION - STACKS ON MOBILE, GRID ON DESKTOP */}
        <div className="row g-3 mb-5">
          <div className="col-12 col-md-4">
            <button 
              className={`cms-nav-btn ${activeSection === 'masters' ? 'active' : ''}`} 
              onClick={() => setActiveSection('masters')}
            >
              <i className="bi bi-database-fill-gear me-3"></i> 
              Masters Management
            </button>
          </div>
          <div className="col-12 col-md-4">
            <button 
              className={`cms-nav-btn ${activeSection === 'users' ? 'active' : ''}`} 
              onClick={() => setActiveSection('users')}
            >
              <i className="bi bi-people-fill me-3"></i> 
              User Management
            </button>
          </div>
          <div className="col-12 col-md-4">
            <button 
              className={`cms-nav-btn ${activeSection === 'jurisdiction' ? 'active' : ''}`} 
              onClick={() => setActiveSection('jurisdiction')}
            >
              <i className="bi bi-pin-map-fill me-3"></i> 
              Jurisdiction Setup
            </button>
          </div>
        </div>

        {/* CONTENT RENDERING SECTION */}
        <div className="section-content">
          {activeSection === 'masters' && renderMasters()}
          {activeSection === 'users' && renderUsers()}
          {activeSection === 'jurisdiction' && renderJurisdictionAssignment()}
        </div>
      </div>

      {/* RESPONSIVE MODAL OVERLAY */}
      {showModal && (
        <div className="modal fade show d-block" style={{ backgroundColor: 'rgba(10, 31, 68, 0.8)' }}>
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content shadow-lg">
              <div className="modal-header border-0 pb-0 px-4 pt-4">
                <h5 className="cms-card-title">
                  {modalType === 'add' ? 'Add New Master Entry' : modalType === 'assign' ? 'Update Jurisdiction' : 'Edit User Access'}
                </h5>
                <button type="button" className="btn-close" onClick={() => setShowModal(false)} aria-label="Close"></button>
              </div>
              <div className="modal-body p-4">
                {modalType === 'assign' ? (
                  <div className="cms-form">
                    <div className="cms-form-group">
                      <label className="cms-label">Officer Selection</label>
                      <select className="cms-select">
                        {MOCK_OFFICERS.map(o => <option key={o.id}>{o.name} - {o.badge}</option>)}
                      </select>
                    </div>
                    <div className="cms-form-group">
                      <label className="cms-label">Target Jurisdiction</label>
                      <select className="cms-select">
                        {MOCK_MASTERS.jurisdictions.map(j => <option key={j.id}>{j.name}</option>)}
                      </select>
                    </div>
                  </div>
                ) : (
                  <div className="cms-form">
                    <div className="cms-form-group">
                      <label className="cms-label">Entry Name</label>
                      <input type="text" className="cms-input" placeholder="e.g. Narcotics, Zone D, etc." />
                    </div>
                    <div className="cms-form-group">
                      <label className="cms-label">Detailed Description</label>
                      <textarea className="cms-textarea" rows="4" placeholder="Brief explanation of the category..."></textarea>
                    </div>
                  </div>
                )}
              </div>
              <div className="modal-footer border-0 p-4 pt-0 d-flex gap-2">
                <button className="cms-btn cms-btn-outline flex-grow-1" onClick={() => setShowModal(false)}>Cancel</button>
                <button className="cms-btn cms-btn-primary flex-grow-1" onClick={() => { setShowModal(false); alert('Successfully Updated System Records.'); }}>
                  Save Changes
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default AdminPage;
