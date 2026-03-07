import React from 'react';
import { Link } from 'react-router-dom';
import '../styles/Dashboard.css';

const StatCard = ({ title, value, icon, color, trend }) => (
  <div className="card stat-card h-100 border-0 shadow-sm overflow-hidden">
    <div className={`card-progress bg-${color}`}></div>
    <div className="card-body d-flex align-items-center p-4">
      <div className={`icon-wrapper bg-${color}-subtle text-${color} me-3`}>
        <i className={`bi bi-${icon}`}></i>
      </div>
      <div>
        <h6 className="card-subtitle mb-1 text-muted text-uppercase small">{title}</h6>
        <h3 className="card-title mb-0 fw-bold">{value}</h3>
        {trend && <span className="small text-success"><i className="bi bi-arrow-up"></i> {trend}</span>}
      </div>
    </div>
  </div>
);

const DashboardPage = () => {
  return (
    <div className="dashboard-page">
      <div className="row mb-4">
        <div className="col">
          <h2 className="fw-bold">Control Center</h2>
          <p className="text-muted">Real-time overview of active crime management data.</p>
        </div>
      </div>

      <div className="row g-4 mb-5">
        <div className="col-12 col-md-6 col-lg-3">
          <StatCard title="Total Complaints" value="1,284" icon="clipboard-data" color="primary" trend="12% from last month" />
        </div>
        <div className="col-12 col-md-6 col-lg-3">
          <StatCard title="Under Investigation" value="342" icon="search" color="warning" />
        </div>
        <div className="col-12 col-md-6 col-lg-3">
          <StatCard title="Evidence Collected" value="5,821" icon="box-seam" color="info" />
        </div>
        <div className="col-12 col-md-6 col-lg-3">
          <StatCard title="Resolved Cases" value="894" icon="check-circle" color="success" />
        </div>
      </div>

      <div className="row g-4">
        <div className="col-lg-8">
          <div className="card shadow-sm border-0 h-100">
            <div className="card-header bg-white py-3 border-0 d-flex justify-content-between align-items-center">
              <h5 className="mb-0 fw-bold">Recent Activities</h5>
              <Link to="/complaints" className="btn btn-sm btn-link">View All</Link>
            </div>
            <div className="card-body">
              <div className="table-responsive">
                <table className="table table-hover align-middle">
                  <thead className="table-light">
                    <tr>
                      <th>Case ID</th>
                      <th>Crime Type</th>
                      <th>Assigned Officer</th>
                      <th>Last Updated</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td>#C1001</td>
                      <td>Vehicle Theft</td>
                      <td>Off. Smith</td>
                      <td>2 hours ago</td>
                    </tr>
                    <tr>
                      <td>#C1002</td>
                      <td>Assault</td>
                      <td>Off. Johnson</td>
                      <td>4 hours ago</td>
                    </tr>
                    <tr>
                      <td>#C1003</td>
                      <td>Vandalism</td>
                      <td>Off. Davis</td>
                      <td>Yesterday</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
        <div className="col-lg-4">
          <div className="card shadow-sm border-0 h-100 bg-primary text-white">
            <div className="card-body p-4 d-flex flex-column justify-content-center text-center">
              <div className="mb-4">
                <i className="bi bi-file-earmark-plus display-4"></i>
              </div>
              <h4>Register New Complaint</h4>
              <p className="small opacity-75">Immediately start a new case record and assign priority.</p>
              <button className="btn btn-light mt-2 fw-bold text-primary">Get Started</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default DashboardPage;
