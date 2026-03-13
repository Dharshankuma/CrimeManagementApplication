import React, { createContext, useContext, useState, useEffect } from 'react';

export const UserRole = {
  ADMIN: 'Admin',
  OFFICER: 'Officer',
  PUBLIC: 'Citizen'
};

const AuthContext = createContext(undefined);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  const isTokenExpired = (token) => {
    if (!token) return true;
    try {
      const payloadBase64 = token.split('.')[1];
      const decodedJson = atob(payloadBase64);
      const decoded = JSON.parse(decodedJson);
      if (!decoded.exp) return false;
      // decoded.exp is in seconds, Date.now() is milliseconds
      return Date.now() >= decoded.exp * 1000;
    } catch (e) {
      return true;
    }
  };

  useEffect(() => {
    const savedUser = localStorage.getItem('crime_system_user');
    const token = localStorage.getItem('crime_system_token');

    if (savedUser && token) {
      if (!isTokenExpired(token)) {
        setUser(JSON.parse(savedUser));
      } else {
        localStorage.removeItem('crime_system_user');
        localStorage.removeItem('crime_system_token');
      }
    }
    setLoading(false);
  }, []);

  const login = (userData, token) => {
    setUser(userData);
    localStorage.setItem('crime_system_user', JSON.stringify(userData));
    localStorage.setItem('crime_system_token', token);
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem('crime_system_user');
    localStorage.removeItem('crime_system_token');
  };

  const checkTokenStatus = () => {
    const token = localStorage.getItem('crime_system_token');
    return !isTokenExpired(token);
  };

  const updateUser = (updates) => {
    const updatedUser = { ...user, ...updates };
    setUser(updatedUser);
    localStorage.setItem('crime_system_user', JSON.stringify(updatedUser));
  };

  return (
    <AuthContext.Provider value={{ user, login, logout, updateUser, loading, checkTokenStatus }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
