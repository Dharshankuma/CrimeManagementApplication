import React, { createContext, useContext, useState, useEffect } from 'react';

export const UserRole = {
  ADMIN: 'Admin',
  OFFICER: 'Officer',
  PUBLIC: 'Public User'
};

const AuthContext = createContext(undefined);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const savedUser = localStorage.getItem('crime_system_user');
    if (savedUser) {
      setUser(JSON.parse(savedUser));
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

  return (
    <AuthContext.Provider value={{ user, login, logout, loading }}>
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
