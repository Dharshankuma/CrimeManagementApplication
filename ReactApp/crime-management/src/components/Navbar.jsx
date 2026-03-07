import React from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth, UserRole } from '../context/AuthContext';

const Navbar = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const isActive = (path) => location.pathname === path ? 'active' : '';

  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark shadow fixed-top" style={{ zIndex: 1050 }}>
      <div className="container">
        <Link className="navbar-brand d-flex align-items-center fw-bold" to="/">
          <i className="bi bi-shield-shaded me-2 text-primary"></i>
          <span className="text-white">CMS</span>
          <span className="ms-2 d-none d-sm-inline opacity-75 small fw-normal">Police Portal</span>
        </Link>
        <button 
          className="navbar-toggler border-0" 
          type="button" 
          data-bs-toggle="collapse" 
          data-bs-target="#navbarNav"
          aria-controls="navbarNav"
          aria-expanded="false" 
          aria-label="Toggle navigation"
        >
          <span className="navbar-toggler-icon"></span>
        </button>
        <div className="collapse navbar-collapse" id="navbarNav">
          <ul className="navbar-nav me-auto mb-2 mb-lg-0">
            <li className="nav-item">
              <Link className={`nav-link ${isActive('/dashboard')}`} to="/dashboard">Dashboard</Link>
            </li>
            <li className="nav-item">
              <Link className={`nav-link ${isActive('/complaints')}`} to="/complaints">Complaints</Link>
            </li>
            <li className="nav-item">
              <Link className={`nav-link ${isActive('/investigations')}`} to="/investigations">Investigations</Link>
            </li>
            {user?.role === UserRole.ADMIN && (
              <li className="nav-item">
                <Link className={`nav-link ${isActive('/admin')}`} to="/admin">
                  <i className="bi bi-gear-fill me-1"></i> Admin Panel
                </Link>
              </li>
            )}
          </ul>
          <div className="d-flex align-items-center gap-3 py-2 py-lg-0 border-top border-secondary border-opacity-25 border-lg-0 mt-2 mt-lg-0">
            <Link to="/profile" className="d-flex align-items-center text-light text-decoration-none hover-opacity">
              <div className="bg-secondary rounded-circle d-flex align-items-center justify-content-center me-2" style={{ width: '32px', height: '32px' }}>
                <i className="bi bi-person h6 mb-0"></i>
              </div>
              <div className="lh-1">
                <div className="small fw-bold">{user?.name}</div>
                <div className="extra-small text-muted">{user?.role}</div>
              </div>
            </Link>
            <button className="btn btn-outline-danger btn-sm px-3" onClick={handleLogout}>
              <i className="bi bi-box-arrow-right me-1"></i> Exit
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
