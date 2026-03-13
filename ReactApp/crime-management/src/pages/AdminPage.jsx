import React, { useState } from 'react';
import '../styles/Admin.css';
import MasterManagement from './admin/MasterManagement';
import UserManagement from './admin/UserManagement';
import JurisdictionSetup from './admin/JurisdictionSetup';

const AdminPage = () => {
  const [activeSection, setActiveSection] = useState('masters');

  return (
    <div className="admin-container">
      <div className="container-fluid">
        <div className="row">
          <div className="col-12">
            <h1 className="cms-title">Admin Control Panel</h1>
            <p className="cms-subtitle">Police System Configuration and Personnel Oversight</p>
          </div>
        </div>

        {/* PRIMARY SECTION NAVIGATION - STACKS ON MOBILE, GRID ON DESKTOP */}
        <div className="row g-3 mb-2">
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
          {activeSection === 'masters' && <MasterManagement />}
          {activeSection === 'users' && <UserManagement />}
          {activeSection === 'jurisdiction' && <JurisdictionSetup />}
        </div>
      </div>
    </div>
  );
};

export default AdminPage;
