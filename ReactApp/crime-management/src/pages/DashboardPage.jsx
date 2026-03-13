import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import AuthService from '../services/AuthService';
import { useAuth, UserRole } from '../context/AuthContext';
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
  const { user } = useAuth();
  const [dashboardData, setDashboardData] = useState(null);
  //const [loading, setLoading] = useState(true);

  const navigate = useNavigate();
  useEffect(() => {
    const fetchDashboardData = async () => {
      try {
        const response = await AuthService.GetServiceCallWithToken("DashBoard/DoGetDashBoardDetails");
        if (response && response.responseStatus === "success") {
          console.log(response)
          setDashboardData(response.data);
        }
      } catch (error) {
        console.error("Error fetching dashboard data:", error);
      } finally {
        //   setLoading(false);
      }
    };

    fetchDashboardData();
  }, []);

  // if (loading) {
  //   return (
  //     <div className="d-flex justify-content-center align-items-center" style={{ minHeight: '60vh' }}>
  //       <div className="spinner-border text-primary" role="status">
  //         <span className="visually-hidden">Loading...</span>
  //       </div>
  //     </div>
  //   );
  // }

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
          <StatCard title="Total Complaints" value={dashboardData?.totalComplaints || 0} icon="clipboard-data" color="primary" trend={`${dashboardData?.overAllStatus || 0}% from last month`} />
        </div>
        <div className="col-12 col-md-6 col-lg-3">
          <StatCard title="Under Investigation" value={dashboardData?.underInvestigation || 0} icon="search" color="warning" />
        </div>
        <div className="col-12 col-md-6 col-lg-3">
          <StatCard title="Evidence Collected" value={dashboardData?.evidenceCollected || 0} icon="box-seam" color="info" />
        </div>
        <div className="col-12 col-md-6 col-lg-3">
          <StatCard title="Resolved Cases" value={dashboardData?.resolvedCases || 0} icon="check-circle" color="success" />
        </div>
      </div>

      <div className="row g-4">
        <div className="col-lg-8">
          <div className="card shadow-sm border-0 h-100">
            <div className="card-header bg-white py-3 border-0 d-flex justify-content-between align-items-center">
              <h5 className="mb-0 fw-bold">Recent Activities</h5>
              {user?.role !== UserRole.OFFICER && <Link to="/complaints" className="btn btn-sm btn-link">View All</Link>}
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
                    {dashboardData?.recentsComplaints && dashboardData.recentsComplaints.length > 0 ? (
                      dashboardData.recentsComplaints.map((complaint, index) => (
                        <tr key={index}>
                          <td>
                            <code className="bg-light px-2 py-1 rounded text-dark border extra-small">
                              ID-{complaint.crimeId ? (complaint.crimeId.toString().length > 10 ? complaint.crimeId.substring(0, 8) : complaint.crimeId) : 'N/A'}
                            </code>
                          </td>
                          <td>{complaint.crimeType}</td>
                          <td>{complaint.ioOfficerName}</td>
                          <td>{complaint.lastUpdated}</td>
                        </tr>
                      ))
                    ) : (
                      <tr>
                        <td colSpan="4" className="text-center">No recent activities found</td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
        {user?.role !== UserRole.OFFICER && (
          <div className="col-lg-4">
            <div className="card shadow-sm border-0 h-100 bg-primary text-white">
              <div className="card-body p-4 d-flex flex-column justify-content-center text-center">
                <div className="mb-4">
                  <i className="bi bi-file-earmark-plus display-4"></i>
                </div>
                <h4>Register New Complaint</h4>
                <p className="small opacity-75">Immediately start a new case record and assign priority.</p>
                <button className="btn btn-light mt-2 fw-bold text-primary" onClick={() => navigate('/complaints/create')}>Get Started</button>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default DashboardPage;
