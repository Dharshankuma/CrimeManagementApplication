import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AuthService from '../../services/AuthService';
import '../../styles/Auth.css';

const ForgotPassword = () => {
  const navigate = useNavigate();
  const [usernameOrEmail, setUsernameOrEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  const [errorMsg, setErrorMsg] = useState('');
  const [successMsg, setSuccessMsg] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setErrorMsg('');
    setSuccessMsg('');

    // Validation
    if (!usernameOrEmail || !password || !confirmPassword) {
      setErrorMsg('Please fill in all fields.');
      return;
    }

    if (password !== confirmPassword) {
      setErrorMsg('Password and Confirm Password must match.');
      return;
    }

    setIsLoading(true);

    const payload = {
      userName: usernameOrEmail,
      emailId: usernameOrEmail,
      password: password,
      confirmPassword: confirmPassword
    };

    try {
      const response = await AuthService.PostServiceCall("Login/ResetPassword", payload);

      // Axios error handling wrapper usually returns { success: false, message: "..." } on failure
      if (response && response.success === false) {
        setErrorMsg(response.message || 'Failed to reset password. Please try again.');
      } else {
        setSuccessMsg('Password reset successfully');
      }
    } catch (error) {
      setErrorMsg('An unexpected error occurred. Please try again later.');
    } finally {
      setIsLoading(false);
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
              <i className="bi bi-shield-lock" style={{ fontSize: '2.5rem' }}></i>
            </div>
            <h3 className="fw-bold mb-0">Reset Password</h3>
            <p className="text-muted small mt-2">Enter your details to reset your password</p>
          </div>

          {/* Messages Section */}
          {errorMsg && (
            <div className="alert alert-danger p-2 text-center shadow-sm" style={{ fontSize: '0.9rem' }}>
              <i className="bi bi-exclamation-triangle-fill me-2"></i>
              {errorMsg}
            </div>
          )}

          {/* Form / Success Section */}
          {!successMsg ? (
            <form onSubmit={handleSubmit}>
              <div className="form-floating mb-3">
                <input
                  type="text"
                  className="form-control"
                  id="resetUsernameOrEmail"
                  placeholder="Username or Email"
                  value={usernameOrEmail}
                  onChange={(e) => setUsernameOrEmail(e.target.value)}
                  required
                />
                <label htmlFor="resetUsernameOrEmail">Username or Email</label>
              </div>

              <div className="form-floating mb-3">
                <input
                  type="password"
                  className="form-control"
                  id="resetPassword"
                  placeholder="New Password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                />
                <label htmlFor="resetPassword">New Password</label>
              </div>

              <div className="form-floating mb-4">
                <input
                  type="password"
                  className="form-control"
                  id="resetConfirmPassword"
                  placeholder="Confirm Password"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  required
                />
                <label htmlFor="resetConfirmPassword">Confirm Password</label>
              </div>

              <button
                type="submit"
                className="btn btn-primary w-100 py-2 mb-4 fw-semibold rounded-3 shadow-sm d-flex align-items-center justify-content-center gap-2"
                disabled={isLoading}
              >
                {isLoading ? (
                  <>
                    <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                    Resetting...
                  </>
                ) : (
                  <>
                    <i className="bi bi-key"></i> Reset Password
                  </>
                )}
              </button>
            </form>
          ) : (
            <div className="alert alert-success text-center mb-4 shadow-sm" role="alert" style={{ fontSize: '0.9rem' }}>
              <i className="bi bi-check-circle-fill d-block mb-2 text-success" style={{ fontSize: '2rem' }}></i>
              {successMsg}
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
