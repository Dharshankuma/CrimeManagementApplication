
import React, { useEffect } from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import { ConfigurationProvider } from './context/ConfigurationContext';
import { LoaderProvider } from './context/LoaderContext';
import Loader from './components/Loader';
import LoginPage from './pages/LoginPage';
import DashboardPage from './pages/DashboardPage';
import ComplaintListPage from './pages/complaints/ComplaintListPage';
import ComplaintCreatePage from './pages/complaints/ComplaintCreatePage';
import InvestigationListPage from './pages/investigation/InvestigationListPage';
import InvestigationDetailsPage from './pages/investigation/InvestigationDetailsPage';
import AdminPage from './pages/AdminPage';
import ProfilePage from './pages/ProfilePage';
import ForgotPassword from './pages/ForgotPassword';
import Navbar from './components/Navbar';
import './styles/Global.css';

const ProtectedRoute = ({ children }) => {
  const { user, loading, logout, checkTokenStatus } = useAuth();

  // Run a status check whenever the user attempts to render a protected component
  useEffect(() => {
    if (user && !checkTokenStatus()) {
      logout();
    }
  }, [user, checkTokenStatus, logout, children]);

  if (loading) return null;
  if (!user || !checkTokenStatus()) return <Navigate to="/login" />;

  return (
    <>
      <Navbar />
      <main className="main-content">
        {children}
      </main>
    </>
  );
};

const App = () => {
  return (
    <LoaderProvider>
      <AuthProvider>
        <ConfigurationProvider>
          <Loader />
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/forgot-password" element={<ForgotPassword />} />
            <Route path="/dashboard" element={<ProtectedRoute><DashboardPage /></ProtectedRoute>} />
            <Route path="/complaints" element={<ProtectedRoute><ComplaintListPage /></ProtectedRoute>} />
            <Route path="/complaints/create" element={<ProtectedRoute><ComplaintCreatePage /></ProtectedRoute>} />
            <Route path="/investigations" element={<ProtectedRoute><InvestigationListPage /></ProtectedRoute>} />
            <Route path="/investigations/:id" element={<ProtectedRoute><InvestigationDetailsPage /></ProtectedRoute>} />
            <Route path="/admin" element={<ProtectedRoute><AdminPage /></ProtectedRoute>} />
            <Route path="/profile" element={<ProtectedRoute><ProfilePage /></ProtectedRoute>} />
            <Route path="/" element={<Navigate to="/dashboard" />} />
          </Routes>
        </ConfigurationProvider>
      </AuthProvider>
    </LoaderProvider>
  );
};

export default App;
