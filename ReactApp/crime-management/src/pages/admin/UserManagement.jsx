import React, { useState, useEffect } from 'react';
import '../../styles/Admin.css';
import AuthService from '../../services/AuthService';
import { useConfiguration } from '../../context/ConfigurationContext';
import Pagination from '../../components/Pagination';

const UserManagement = () => {
  const [users, setUsers] = useState([]);
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize] = useState(10);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [alert, setAlert] = useState({ show: false, message: '', type: 'danger' });

  const showAlert = (message, type = 'danger') => {
    setAlert({ show: true, message, type });
    setTimeout(() => setAlert({ show: false, message: '', type: 'danger' }), 5000);
  };

  const [selectedUser, setSelectedUser] = useState(null);
  const [selectedRole, setSelectedRole] = useState("");
  const [selectedJurisdiction, setSelectedJurisdiction] = useState("");

  const { configuration } = useConfiguration();
  const roles = configuration?.roleMaster || [];
  const jurisdictions = configuration?.jurisdictionMaster || [];

  const fetchUsers = async () => {
    setLoading(true);
    try {
      const response = await AuthService.PostServiceCallTokenWithToken(
        "User/GetUser",
        {
          pageNumber: pageNumber,
          pageSize: pageSize
        }
      );

      if (response && response.responseCode === 200 && response.data) {
        setUsers(response.data.data || []);
        setTotalCount(response.data.totalCount || 0);
      } else {
        setUsers([]);
        setTotalCount(0);
      }
    } catch (error) {
      console.error("Failed to fetch users", error);
      setUsers([]);
      setTotalCount(0);
    } finally {
      setLoading(false);
    }
  };

  const updateUser = async () => {
    try {
      await AuthService.PostServiceCallTokenWithToken(
        "User/AddUser",
        {
          identifier: selectedUser.identifier,
          roleIdentifier: selectedRole,
          jurisdictionIdentifier: selectedJurisdiction
        }
      );

      setShowModal(false);
      fetchUsers();
    } catch (error) {
      console.error("Failed to update user", error);
      showAlert("Failed to update user.", "danger");
    }
  };

  useEffect(() => {
    fetchUsers();
  }, [pageNumber]);

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
          <div className="table-responsive mt-3">
            {loading ? (
              <div className="text-center p-4">Loading...</div>
            ) : (
              <>
                <table className="cms-table">
                  <thead>
                    <tr>
                      <th>User Identity</th>
                      <th className="d-none d-md-table-cell">Contact Info</th>
                      <th>System Role</th>
                      <th className="text-end">Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {users.map(user => (
                      <tr key={user.identifier}>
                        <td>
                          <div className="fw-bold">{user.firstName} {user.lastName}</div>
                          <div className="extra-small text-muted">{user.identifier}</div>
                        </td>
                        <td className="d-none d-md-table-cell">
                          <div className="small"><i className="bi bi-envelope me-1"></i> {user.emailId}</div>
                          <div className="small"><i className="bi bi-phone me-1"></i> {user.phoneNo}</div>
                        </td>
                        <td>
                          <span className="badge border text-dark fw-bold px-2 py-1" style={{ backgroundColor: '#F3F4F6' }}>
                            {user.roleName}
                          </span>
                        </td>
                        <td>
                          <div className="d-flex gap-2 justify-content-end">
                            {/* <button className="cms-btn cms-btn-outline btn-sm"><i className="bi bi-eye"></i></button> */}
                            <button
                              className="cms-btn cms-btn-outline btn-sm"
                              onClick={() => {
                                setSelectedUser(user);
                                setSelectedRole(user.roleIdentifier);
                                setSelectedJurisdiction(user.jurisdictionIdentifier);
                                setShowModal(true);
                              }}
                            >
                              <i className="bi bi-pencil-square"></i>
                            </button>
                          </div>
                        </td>
                      </tr>
                    ))}
                    {users.length === 0 && (
                      <tr>
                        <td colSpan="4" className="text-center text-muted py-4">No users found.</td>
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
                <h5 className="cms-card-title">Update User Assignment</h5>
                <button type="button" className="btn-close" onClick={() => setShowModal(false)} aria-label="Close"></button>
              </div>
              <div className="modal-body p-4">
                <div className="cms-form">
                  <div className="cms-form-group">
                    <label className="cms-label">Role</label>
                    <select
                      className="cms-select"
                      value={selectedRole}
                      onChange={(e) => setSelectedRole(e.target.value)}
                    >
                      <option value="">Select Role</option>
                      {roles.map(role => (
                        <option key={role.identifier} value={role.identifier}>
                          {role.name}
                        </option>
                      ))}
                    </select>
                  </div>
                  <div className="cms-form-group">
                    <label className="cms-label">Jurisdiction</label>
                    <select
                      className="cms-select"
                      value={selectedJurisdiction}
                      onChange={(e) => setSelectedJurisdiction(e.target.value)}
                    >
                      <option value="">Select Jurisdiction</option>
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
                <button className="cms-btn cms-btn-primary flex-grow-1" onClick={updateUser}>
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

export default UserManagement;
