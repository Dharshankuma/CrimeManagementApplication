import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import StatusBadge from '../../components/StatusBadge';
import { useAuth } from '../../context/AuthContext';
import { useConfiguration } from '../../context/ConfigurationContext';
import { useLoader } from '../../context/LoaderContext';
import AuthService from '../../services/AuthService';

const ComplaintListPage = () => {
  const navigate = useNavigate();
  const { user } = useAuth();
  const { configuration } = useConfiguration();
  const { setLoading } = useLoader();

  // State
  const [complaints, setComplaints] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [crimeTypeFilter, setCrimeTypeFilter] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 10;

  const fetchComplaints = useCallback(async () => {
    // Only fetch if user is defined
    // Use id or identifier based on typical backend JWT mappings
    const activeUserId = user?.identifier || '';

    try {
      setLoading(true);

      const payload = {
        crimeType: crimeTypeFilter || "",
        search: searchTerm || "",
        reportDate: "",
        columnName: "",
        sortOrder: true,
        crimeIdentifier: "",
        userIdentifier: activeUserId,
        pageNumber: currentPage,
        pageSize: pageSize
      };

      console.log("Fetching complaints with payload:", payload);

      // Call API using POST service method
      const response = await AuthService.PostServiceCallToken('CrimeReport/GetCrimeReports', payload);
      console.log("Complaints API Response:", response);

      if (response && response.success !== false) {
        // Depending on API wrapping behavior, the result could be directly inside data or data.data etc.
        // Handle standard REST payload wraps
        let responseData = response.data?.data || response.data || response || [];

        if (Array.isArray(responseData)) {
          setComplaints(responseData);
        } else if (responseData && Array.isArray(responseData.items)) {
          setComplaints(responseData.items);
        } else {
          setComplaints([]);
        }
      } else {
        setComplaints([]);
        console.error("Failed to fetch complaints:", response?.message);
      }
    } catch (error) {
      console.error("Error fetching complaints payload:", error);
      setComplaints([]);
    } finally {
      setLoading(false);
    }
  }, [crimeTypeFilter, searchTerm, currentPage, user, setLoading]);

  // Load complaints on mount and when filters/page change
  useEffect(() => {
    // Basic debounce structure for smooth typing
    const timeoutId = setTimeout(() => {
      fetchComplaints();
    }, 500);

    return () => clearTimeout(timeoutId);
  }, [fetchComplaints]);

  // Handle refresh explicitly
  const handleRefresh = () => {
    fetchComplaints();
  };

  return (
    <div className="complaint-list">

      <div>
        <h2 className="fw-bold mb-1">Complaints Management</h2>
      </div>

      <div className="d-flex flex-column flex-md-row justify-content-between align-items-md-center mt-4 mb-4 gap-3">

        <div className="d-flex gap-2 align-items-center">
          {/* Refresh Button */}
          <button
            type="button"
            className="btn btn-outline-secondary"
            onClick={handleRefresh}
            title="Refresh Data"
          >
            <i className="bi bi-arrow-clockwise"></i>
          </button>

          {/* Crime Type Dropdown Filter */}
          <select
            className="form-select"
            style={{ maxWidth: '180px' }}
            value={crimeTypeFilter}
            onChange={(e) => {
              setCrimeTypeFilter(e.target.value);
              setCurrentPage(1); // Reset page to 1 on filter
            }}
          >
            <option value="">All Crime Types</option>
            {configuration?.crimeTypes?.map((ct) => (
              <option key={ct.identifier} value={ct.identifier}>
                {ct.name}
              </option>
            ))}
          </select>

          {/* Search Input */}
          <div className="input-group" style={{ maxWidth: '250px' }}>
            <span className="input-group-text bg-white border-end-0"><i className="bi bi-search"></i></span>
            <input
              type="text"
              className="form-control border-start-0 ps-0"
              placeholder="Search by name or ID..."
              value={searchTerm}
              onChange={(e) => {
                setSearchTerm(e.target.value);
                setCurrentPage(1); // Reset page to 1 on search
              }}
            />
          </div>

          {/* Create Complaint Button */}

        </div>

        <button
          className="btn btn-primary text-nowrap"
          onClick={() => navigate('/complaints/create')}
        >
          <i className="bi bi-plus-lg me-1"></i> New
        </button>
      </div>

      <div className="card shadow-sm border-0">
        <div className="card-body p-0">
          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead className="table-light">
                <tr>
                  <th className="ps-4 py-3">Report ID / Complaint Name</th>
                  <th className="py-3">Jurisdiction</th>
                  <th className="py-3">Crime Type</th>
                  <th className="py-3">Status</th>
                  <th className="py-3">Investigating Officer</th>
                  <th className="py-3">Dates</th>
                  <th className="pe-4 py-3 text-end">Action</th>
                </tr>
              </thead>
              <tbody>
                {complaints.length > 0 ? (
                  complaints.map((complaint) => (
                    <tr key={complaint.reportIdentifer || complaint.reportId}>
                      <td className="ps-4">
                        <div className="fw-bold">{complaint.complaintName || "Unknown"}</div>
                        <div className="text-muted small">ID: {complaint.reportId || "N/A"}</div>
                      </td>
                      <td>{complaint.jurisdictionName || "-"}</td>
                      <td>{complaint.crimeType || "-"}</td>
                      <td>
                        <StatusBadge status={complaint.crimeStatusStr || complaint.crimeStatus} />
                      </td>
                      <td>{complaint.ioOfficerName || "Unassigned"}</td>
                      <td>
                        <div>{complaint.crimeReportDate ? new Date(complaint.crimeReportDate).toLocaleDateString() : "-"}</div>
                        <div className="text-muted small">Updated: {complaint.lastUpdated ? new Date(complaint.lastUpdated).toLocaleDateString() : "-"}</div>
                      </td>
                      <td className="pe-4 text-end">
                        <button
                          className="btn btn-sm btn-outline-primary"
                          onClick={() => navigate(`/investigation-details/${complaint.reportIdentifer}`)}
                        >
                          View Details
                        </button>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan="7" className="text-center py-4 text-muted">
                      No complaints found matching your criteria.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
        <div className="card-footer bg-white border-0 py-3">
          <nav>
            <ul className="pagination pagination-sm justify-content-center mb-0">
              <li className={`page-item ${currentPage <= 1 ? 'disabled' : ''}`}>
                <button
                  className="page-link"
                  onClick={() => setCurrentPage(prev => Math.max(prev - 1, 1))}
                  disabled={currentPage <= 1}
                >
                  Previous
                </button>
              </li>
              <li className="page-item active">
                <span className="page-link">{currentPage}</span>
              </li>
              <li className={`page-item ${complaints.length < pageSize ? 'disabled' : ''}`}>
                <button
                  className="page-link"
                  onClick={() => setCurrentPage(prev => prev + 1)}
                  disabled={complaints.length < pageSize}
                >
                  Next
                </button>
              </li>
            </ul>
          </nav>
        </div>
      </div>
    </div>
  );
};

export default ComplaintListPage;
