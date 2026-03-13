import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import StatusBadge from '../../components/StatusBadge';
import AuthService from '../../services/AuthService';
import { useAuth } from '../../context/AuthContext';
import { useLoader } from '../../context/LoaderContext';
import Pagination from '../../components/Pagination';

const InvestigationListPage = () => {
  const navigate = useNavigate();
  const { user } = useAuth();
  const { setLoading } = useLoader();

  const [investigations, setInvestigations] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const pageSize = 10;

  const fetchInvestigations = useCallback(async () => {
    try {
      setLoading(true);
      const payload = {
        search: searchTerm || "",
        columnName: "",
        sortOrder: true,
        userIdentifier: user?.identifier || "",
        pageNumber: currentPage,
        pageSize: pageSize
      };

      const apiCall = AuthService.PostServiceCallToken;
      const response = await apiCall("Investigation/InvestigationGridDetails", payload);

      console.log(response);
      let fetchedTotal = response?.totalCount || (response?.data && response.data.totalCount) || 0;
      console.log(response.data);
      if (response && response.data && response.data.data) {
        setInvestigations(response.data.data);
        setTotalCount(fetchedTotal > 0 ? fetchedTotal : response.data.data.length);
      } else if (Array.isArray(response)) {
        setInvestigations(response);
        setTotalCount(fetchedTotal > 0 ? fetchedTotal : response.length);
      } else if (response && Array.isArray(response.data)) {
        setInvestigations(response.data);
        setTotalCount(fetchedTotal > 0 ? fetchedTotal : response.data.length);
      } else {
        setInvestigations([]);
        setTotalCount(0);
      }
    } catch (error) {
      console.error("Error fetching investigations:", error);
      setInvestigations([]);
      setTotalCount(0);
    } finally {
      setLoading(false);
    }
  }, [searchTerm, currentPage, user, setLoading, pageSize]);

  useEffect(() => {
    const timeoutId = setTimeout(() => {
      fetchInvestigations();
    }, 500);

    return () => clearTimeout(timeoutId);
  }, [fetchInvestigations]);

  const handleRefresh = () => {
    fetchInvestigations();
  };

  const getPriorityClasses = (inv) => {
    const priority = (inv.priority || "").toUpperCase();
    if (priority === 'HIGH' || inv.statusId === 1) {
      return { text: 'HIGH', className: 'bg-danger-subtle text-danger' };
    }
    if (priority === 'LOW' || inv.statusId === 3) {
      return { text: 'LOW', className: 'bg-info-subtle text-info' };
    }
    return { text: 'MEDIUM', className: 'bg-warning-subtle text-warning' };
  };

  const formatDate = (dateString) => {
    if (!dateString) return "N/A";
    const options = { year: 'numeric', month: 'short', day: 'numeric' };
    return new Date(dateString).toLocaleDateString(undefined, options);
  };

  return (
    <div className="investigation-list">
      <div className="mb-4">
        <h2 className="fw-bold mb-1">Active Investigations</h2>
        <p className="text-muted">Ongoing cases currently assigned to field officers.</p>
      </div>

      <div className="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 gap-3">
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
        </div>
      </div>

      <div className="card shadow-sm border-0 overflow-hidden">
        <div className="table-responsive">
          <table className="table table-hover align-middle mb-0">
            <thead className="bg-light">
              <tr>
                <th className="ps-4 py-3">Investigation ID</th>
                <th>Status</th>
                <th>Assigned Officer</th>
                <th>Last Update</th>
                <th className="pe-4 text-end">Action</th>
              </tr>
            </thead>
            <tbody>
              {investigations.map((inv, idx) => {
                const priorityData = getPriorityClasses(inv);
                const progressWidth = Math.min(100, 60 + (idx * 15)); // Keep static progress aesthetic if desired, or link to inv later

                return (
                  <tr key={inv.investigationIdentifer || idx}>
                    <td className="ps-4">
                      <div className="fw-bold">{inv.complaintName}</div>
                      <div className="extra-small text-muted mt-1">
                        <code className="bg-light px-2 py-1 rounded text-dark border">
                          CASE-{inv.complaintIdentifier ? inv.complaintIdentifier.substring(0, 8) : 'N/A'}
                        </code>
                      </div>
                    </td>
                    <td>
                      <StatusBadge status={inv.crimeStatus} />
                    </td>
                    <td>{inv.ioOfficerName || 'Unassigned'}</td>
                    <td>{inv.lastUpdatedDate}</td>
                    <td className="pe-4 text-end">
                      <button
                        className="btn btn-sm btn-primary"
                        onClick={() => navigate(`/investigations/${inv.complaintIdentifier}`)}
                      >
                        View Case
                      </button>
                    </td>
                  </tr>
                );
              })}

              {investigations.length === 0 && (
                <tr>
                  <td colSpan="6" className="text-center py-4 text-muted">
                    No investigations found matching your criteria.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        {/* Pagination Controls */}
        <Pagination
          currentPage={currentPage}
          pageSize={pageSize}
          totalCount={totalCount > 0 ? totalCount : (currentPage * pageSize + (investigations.length === pageSize ? 1 : 0))}
          onPageChange={setCurrentPage}
        />
      </div>
    </div>
  );
};

export default InvestigationListPage;
