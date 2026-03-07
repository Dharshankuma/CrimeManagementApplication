import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth, UserRole } from '../context/AuthContext';
import AuthService from '../services/AuthService';
import '../styles/Auth.css';

const LoginPage = () => {
  const { login } = useAuth();
  const navigate = useNavigate();

  // Tab state
  const [activeTab, setActiveTab] = useState('login'); // 'login' or 'register'

  // Input states
  const [loginForm, setLoginForm] = useState({
    identifier: '',
    password: ''
  });

  const [registerForm, setRegisterForm] = useState({
    fullName: '',
    email: '',
    password: ''
  });

  const [errorMsg, setErrorMsg] = useState(null);
  const [successMsg, setSuccessMsg] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  // Handlers
  const handleGoogleCallback = async (response) => {
    console.log(response);
    setErrorMsg(null);
    setSuccessMsg(null);
    setIsLoading(true);

    try {
      const apiResponse = await AuthService.PostServiceCall("Login/LoginWithGoogle", {
        googleToken: response.credential
      });

      setIsLoading(false);

      if (apiResponse.success === false) {
        setErrorMsg(apiResponse.message || "Google Login failed. Please try again.");
      } else if (apiResponse.responseStatus === "success") {
        const token = apiResponse.data.token;
        const user = apiResponse.data.userDetails;
        login(user, token);
        navigate('/dashboard');
      } else {
        setErrorMsg(apiResponse.responseMessage || "Google Login failed");
      }
    } catch (error) {
      setIsLoading(false);
      setErrorMsg("An unexpected error occurred during Google Login.");
    }
  };

  useEffect(() => {
    if (window.google) {
      window.google.accounts.id.initialize({
        // NOTE: Replace with your actual Google Client ID below
        client_id: "800773409194-9gbkps7ahlrlsr0jnq8v89irjfvm50uu.apps.googleusercontent.com",
        callback: handleGoogleCallback
      });

      const googleBtn = document.getElementById("googleSignInDiv");
      if (googleBtn) {
        window.google.accounts.id.renderButton(
          googleBtn,
          { theme: "outline", size: "large", width: 350 }
        );
      }
    }
  }, [activeTab]);

  const handleLoginSubmit = async (e) => {
    e.preventDefault();
    setErrorMsg(null);
    setSuccessMsg(null);

    if (loginForm.identifier && loginForm.password) {
      setIsLoading(true);
      const response = await AuthService.PostServiceCall("Login/LoginUser", {
        userName: loginForm.identifier,
        password: loginForm.password
      });
      setIsLoading(false);

      if (response.success === false) {
        setErrorMsg(response.message || "Invalid credentials. Please try again.");
      } else if (response.responseStatus === "success") {
        const token = response.data.token;
        const user = response.data.userDetails;
        login(user, token);
        navigate('/dashboard');
      } else {
        setErrorMsg(response.responseMessage || "Login failed");
      }
    }
  };

  const handleRegisterSubmit = async (e) => {
    e.preventDefault();
    setErrorMsg(null);
    setSuccessMsg(null);

    if (registerForm.fullName && registerForm.email && registerForm.password) {
      setIsLoading(true);
      const response = await AuthService.PostServiceCall("Login/RegisterUser", {
        userName: registerForm.fullName,
        emailId: registerForm.email,
        password: registerForm.password
      });
      setIsLoading(false);

      if (response.success === false) {
        setErrorMsg(response.message || "Registration failed. Please try again.");
      } else if (response.responseStatus === "success") {
        setSuccessMsg("Registration successful! Please login.");
        setRegisterForm({ fullName: '', email: '', password: '' });
        setActiveTab('login');
      } else {
        setErrorMsg(response.responseMessage || "Registration failed");
      }
    }
  };



  return (
    <div className="login-wrapper d-flex align-items-center justify-content-center min-vh-100 p-3">
      <div className="card shadow-lg border-0 login-card" style={{ maxWidth: '400px', width: '100%', borderRadius: '1rem' }}>
        <div className="card-body p-4 p-md-5">

          {/* Brand / Logo Section */}
          <div className="text-center mb-4">
            <div className="bg-primary d-inline-flex align-items-center justify-content-center rounded-circle text-white mb-3 shadow" style={{ width: '70px', height: '70px' }}>
              <i className="bi bi-shield-check" style={{ fontSize: '2.5rem' }}></i>
            </div>
            <h3 className="fw-bold mb-0">CMS Portal</h3>
            <p className="text-muted small">Police Crime Management System</p>
          </div>

          {/* Toggle Tabs */}
          <div className="d-flex mb-4 p-1 bg-light rounded" style={{ border: '1px solid #e2e8f0' }}>
            <button
              type="button"
              className={`flex-fill border-0 p-2 rounded ${activeTab === "login"
                ? "bg-dark text-white shadow-sm fw-bold"
                : "bg-transparent text-muted fw-medium"
                }`}
              style={{ transition: 'all 0.2s' }}
              onClick={() => setActiveTab("login")}
            >
              Login
            </button>

            <button
              type="button"
              className={`flex-fill border-0 p-2 rounded ${activeTab === "register"
                ? "bg-dark text-white shadow-sm fw-bold"
                : "bg-transparent text-muted fw-medium"
                }`}
              style={{ transition: 'all 0.2s' }}
              onClick={() => setActiveTab("register")}
            >
              Register
            </button>
          </div>

          {/* Error and Success Messages */}
          {errorMsg && (
            <div className="alert alert-danger py-2 px-3 small shadow-sm d-flex align-items-center gap-2 mb-4" role="alert">
              <i className="bi bi-exclamation-triangle-fill"></i> {errorMsg}
            </div>
          )}

          {successMsg && (
            <div className="alert alert-success py-2 px-3 small shadow-sm d-flex align-items-center gap-2 mb-4" role="alert">
              <i className="bi bi-check-circle-fill"></i> {successMsg}
            </div>
          )}

          {/* Tab Content */}
          <div className="tab-content">

            {/* LOGIN TAB */}
            {activeTab === 'login' && (
              <div className="tab-pane fade show active">
                <form onSubmit={handleLoginSubmit}>
                  <div className="form-floating mb-3">
                    <input
                      type="text"
                      className="form-control"
                      id="loginIdentifier"
                      placeholder="Username or Email"
                      value={loginForm.identifier}
                      onChange={(e) => setLoginForm({ ...loginForm, identifier: e.target.value })}
                      required
                    />
                    <label htmlFor="loginIdentifier">Username or Email</label>
                  </div>

                  <div className="form-floating mb-1">
                    <input
                      type="password"
                      className="form-control"
                      id="loginPassword"
                      placeholder="Password"
                      value={loginForm.password}
                      onChange={(e) => setLoginForm({ ...loginForm, password: e.target.value })}
                      required
                    />
                    <label htmlFor="loginPassword">Password</label>
                  </div>

                  <div className="d-flex justify-content-end mb-4">
                    <button
                      type="button"
                      onClick={() => navigate('/forgot-password')}
                      className="btn btn-link text-decoration-none p-0 small text-primary fw-medium"
                    >
                      Forgot Password?
                    </button>
                  </div>

                  <button type="submit" disabled={isLoading} className="btn btn-primary w-100 py-2 mb-3 fw-semibold rounded-3 shadow-sm d-flex justify-content-center align-items-center gap-2">
                    {isLoading ? (
                      <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                    ) : (
                      "Login"
                    )}
                  </button>

                  <div className="d-flex align-items-center mb-3">
                    <hr className="flex-grow-1 bg-secondary" />
                    <span className="px-3 text-muted small fw-medium">OR</span>
                    <hr className="flex-grow-1 bg-secondary" />
                  </div>

                  <div id="googleSignInDiv" className="d-flex justify-content-center w-100"></div>
                </form>
              </div>
            )}

            {/* REGISTER TAB */}
            {activeTab === 'register' && (
              <div className="tab-pane fade show active">
                <form onSubmit={handleRegisterSubmit}>
                  <div className="form-floating mb-3">
                    <input
                      type="text"
                      className="form-control"
                      id="registerName"
                      placeholder="Full Name"
                      value={registerForm.fullName}
                      onChange={(e) => setRegisterForm({ ...registerForm, fullName: e.target.value })}
                      required
                    />
                    <label htmlFor="registerName">Full Name</label>
                  </div>

                  <div className="form-floating mb-3">
                    <input
                      type="email"
                      className="form-control"
                      id="registerEmail"
                      placeholder="Email Address"
                      value={registerForm.email}
                      onChange={(e) => setRegisterForm({ ...registerForm, email: e.target.value })}
                      required
                    />
                    <label htmlFor="registerEmail">Email Address</label>
                  </div>

                  <div className="form-floating mb-4">
                    <input
                      type="password"
                      className="form-control"
                      id="registerPassword"
                      placeholder="Password"
                      value={registerForm.password}
                      onChange={(e) => setRegisterForm({ ...registerForm, password: e.target.value })}
                      required
                    />
                    <label htmlFor="registerPassword">Password</label>
                  </div>

                  <button type="submit" disabled={isLoading} className="btn btn-primary w-100 py-2 mb-3 fw-semibold rounded-3 shadow-sm d-flex justify-content-center align-items-center gap-2">
                    {isLoading ? (
                      <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                    ) : (
                      "Register"
                    )}
                  </button>

                  <div className="d-flex align-items-center mb-3">
                    <hr className="flex-grow-1 bg-secondary" />
                    <span className="px-3 text-muted small fw-medium">OR</span>
                    <hr className="flex-grow-1 bg-secondary" />
                  </div>

                  <div id="googleSignInDiv" className="d-flex justify-content-center w-100"></div>
                </form>
              </div>
            )}

          </div>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
