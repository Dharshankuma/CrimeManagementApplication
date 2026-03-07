import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/Auth.css';

const ForgotPassword = () => {
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [isSubmitted, setIsSubmitted] = useState(false);

  const handleSubmit = (e) => {
    e.preventDefault();
    if (email) {
      // Simulate API call for password reset
      setIsSubmitted(true);
    }
  };

  const handleBackToLogin = () => {
    navigate('/login');
  };

  return (
    <div className="login-wrapper bg-light d-flex align-items-center justify-content-center min-vh-100 p-3">
      <div className="card shadow-lg border-0 login-card" style={{ maxWidth: '400px', width: '100%', borderRadius: '1rem' }}>
        <div className="card-body p-4 p-md-5">
          
          {/* Header Section */}
          <div className="text-center mb-4">
            <div className="bg-primary d-inline-flex align-items-center justify-content-center rounded-circle text-white mb-3 shadow" style={{ width: '70px', height: '70px' }}>
              <i className="bi bi-shield-check" style={{ fontSize: '2.5rem' }}></i>
            </div>
            <h3 className="fw-bold mb-0">Forgot Password</h3>
            <p className="text-muted small mt-2">Enter your email to receive a password reset link</p>
          </div>

          {/* Form / Success Message Section */}
          {!isSubmitted ? (
            <form onSubmit={handleSubmit}>
              <div className="form-floating mb-4">
                <input
                  type="email"
                  className="form-control"
                  id="resetEmail"
                  placeholder="Email Address"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
                <label htmlFor="resetEmail">Email Address</label>
              </div>
              
              <button type="submit" className="btn btn-primary w-100 py-2 mb-4 fw-semibold rounded-3 shadow-sm d-flex align-items-center justify-content-center gap-2">
                <i className="bi bi-envelope-check"></i> Send Reset Link
              </button>
            </form>
          ) : (
            <div className="alert alert-success text-center mb-4 shadow-sm" role="alert" style={{ fontSize: '0.9rem' }}>
              <i className="bi bi-check-circle-fill d-block mb-2 text-success" style={{ fontSize: '2rem' }}></i>
              If an account exists with this email, a password reset link has been sent.
            </div>
          )}

          {/* Navigation */}
          <div className="text-center mt-2 border-top pt-3">
            <button 
              type="button" 
              onClick={handleBackToLogin}
              className="btn btn-link text-decoration-none text-muted p-0 d-inline-flex align-items-center gap-2 fw-medium"
            >
              <i className="bi bi-arrow-left"></i> Back to Login
            </button>
          </div>
          
        </div>
      </div>
    </div>
  );
};

export default ForgotPassword;
