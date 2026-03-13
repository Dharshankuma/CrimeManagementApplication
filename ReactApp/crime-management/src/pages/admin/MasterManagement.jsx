import React, { useState, useEffect } from 'react';
import '../../styles/Admin.css';
import AuthService from '../../services/AuthService';
import Pagination from '../../components/Pagination';

const MasterManagement = () => {
  const [activeMasterTab, setActiveMasterTab] = useState('crimeTypes');
  const [showModal, setShowModal] = useState(false);
  const [modalType, setModalType] = useState('');

  const [masterData, setMasterData] = useState([]);
  const [editItem, setEditItem] = useState(null);
  const [editName, setEditName] = useState("");
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize] = useState(5);
  const [loading, setLoading] = useState(false);
  const [alert, setAlert] = useState({ show: false, message: '', type: 'danger' });

  const showAlert = (message, type = 'danger') => {
    setAlert({ show: true, message, type });
    setTimeout(() => setAlert({ show: false, message: '', type: 'danger' }), 5000);
  };

  useEffect(() => {
    setPageNumber(0);
  }, [activeMasterTab]);

  const updateMaster = async () => {
    let adminType = 1;

    if (activeMasterTab === "crimeTypes") adminType = 1;
    if (activeMasterTab === "status") adminType = 2;
    if (activeMasterTab === "jurisdictions") adminType = 3;

    try {
      await AuthService.PostServiceCallTokenWithToken(
        "/UpdateMasterDetails",
        {
          identifier: editItem.identifier,
          name: editName,
          adminType: adminType
        }
      );

      setShowModal(false);
      fetchMasterData(adminType, pageNumber);
    } catch (error) {
      console.error("Failed to update master data", error);
      showAlert("Failed to update record.", "danger");
    }
  };

  const deleteMaster = async (item) => {
    let adminType = 1;

    if (activeMasterTab === "crimeTypes") adminType = 1;
    if (activeMasterTab === "status") adminType = 2;
    if (activeMasterTab === "jurisdictions") adminType = 3;

    const confirmDelete = window.confirm("Are you sure you want to delete this record?");
    if (!confirmDelete) return;

    try {
      await AuthService.Deleteserivecall(
        "/DeleteMasterDetails",
        {
          identifier: item.identifier,
          adminType: adminType
        }
      );

      let newPageNum = pageNumber;
      if (masterData.length === 1 && pageNumber > 0) {
        setPageNumber(pageNumber - 1);
        newPageNum = pageNumber - 1;
      }
      fetchMasterData(adminType, newPageNum);
      showAlert("Record deleted successfully.", "success");
    } catch (error) {
      console.error("Failed to delete record", error);
      showAlert("Failed to delete record.", "danger");
    }
  };

  const fetchMasterData = async (adminType, pageNum) => {
    setLoading(true);
    try {
      const response = await AuthService.PostServiceCallTokenWithToken(
        "Admin/GetMasterDetails",
        {
          adminType: adminType,
          pageNumber: pageNum,
          pageSize: pageSize
        }
      );
      if (response && response.responseCode === 200 && response.data) {
        setMasterData(response.data.gridDetails || []);
        setTotalCount(response.data.totalCount || 0);
      } else {
        setMasterData([]);
        setTotalCount(0);
      }
    } catch (error) {
      console.error("Failed to fetch master data", error);
      setMasterData([]);
      setTotalCount(0);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    let type = 1;
    if (activeMasterTab === "crimeTypes") type = 1;
    if (activeMasterTab === "status") type = 2;
    if (activeMasterTab === "jurisdictions") type = 3;

    fetchMasterData(type, pageNumber);
  }, [activeMasterTab, pageNumber]);

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
              </div>
            </div>
          </div>
          <div className="table-responsive">
            {loading ? (
              <div className="p-4 text-center">Loading...</div>
            ) : (
              <>
                <table className="cms-table">
                  <thead>
                    <tr>
                      <th>ID</th>
                      <th>Name</th>
                      <th>Status</th>
                      <th className="text-end">Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {masterData.map((item, index) => (
                      <tr key={index}>
                        <td>{pageNumber * pageSize + index + 1}</td>
                        <td className="fw-bold">{item.name}</td>
                        <td>
                          <span className={`cms-status-badge ${item.status === 1 ? 'status-active' : 'status-inactive'}`}>
                            {item.status === 1 ? 'Active' : 'Inactive'}
                          </span>
                        </td>
                        <td className="text-end">
                          <div className="d-flex gap-2 justify-content-end">
                            <button
                              className="cms-btn cms-btn-outline btn-sm"
                              onClick={() => {
                                setEditItem(item);
                                setEditName(item.name);
                                setModalType('edit');
                                setShowModal(true);
                              }}
                            >
                              <i className="bi bi-pencil"></i>
                            </button>
                            <button
                              className="cms-btn cms-btn-danger-outline btn-sm"
                              onClick={() => deleteMaster(item)}
                            >
                              <i className="bi bi-trash"></i>
                            </button>
                          </div>
                        </td>
                      </tr>
                    ))}
                    {masterData.length === 0 && (
                      <tr>
                        <td colSpan="4" className="text-center text-muted py-4">No master data found.</td>
                      </tr>
                    )}
                  </tbody>
                </table>


              </>
            )}
          </div>
          <Pagination 
            currentPage={pageNumber + 1} 
            pageSize={pageSize} 
            totalCount={totalCount} 
            onPageChange={(page) => setPageNumber(page - 1)} 
          />
        </div>
      </div>

      {showModal && (
        <div className="modal fade show d-block" style={{ backgroundColor: 'rgba(10, 31, 68, 0.8)' }}>
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content shadow-lg">
              <div className="modal-header border-0 pb-0 px-4 pt-4">
                <h5 className="cms-card-title">Edit Master Entry</h5>
                <button type="button" className="btn-close" onClick={() => setShowModal(false)} aria-label="Close"></button>
              </div>
              <div className="modal-body p-4">
                <div className="cms-form">
                  <div className="cms-form-group">
                    <label className="cms-label">Entry Name</label>
                    <input
                      type="text"
                      className="cms-input"
                      value={editName}
                      onChange={(e) => setEditName(e.target.value)}
                    />
                  </div>
                </div>
              </div>
              <div className="modal-footer border-0 p-4 pt-0 d-flex gap-2">
                <button className="cms-btn cms-btn-outline flex-grow-1" onClick={() => setShowModal(false)}>Cancel</button>
                <button className="cms-btn cms-btn-primary flex-grow-1" onClick={() => updateMaster()}>
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

export default MasterManagement;
