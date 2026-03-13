import React, { createContext, useContext, useState, useEffect } from 'react';
import AuthService from '../services/AuthService';
import { useAuth } from './AuthContext';

// 1. Create the Context
const ConfigurationContext = createContext();

// 2. Custom hook for consuming the configuration context
export const useConfiguration = () => {
    const context = useContext(ConfigurationContext);
    if (!context) {
        throw new Error('useConfiguration must be used within a ConfigurationProvider');
    }
    return context;
};

// 3. Provider Component
export const ConfigurationProvider = ({ children }) => {
    const [configuration, setConfiguration] = useState({
        countryMaster: [],
        stateMaster: [],
        jurisdictionMaster: [],
        crimeTypes: [],
        statusMaster: [],
        roleMaster: []
    });
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const { user } = useAuth();

    useEffect(() => {
        let isMounted = true;

        const fetchConfig = async () => {
            // Only fetch configuration if the user is logged in
            if (!user) {
                if (isMounted) {
                    setLoading(false);
                }
                return;
            }

            try {
                setLoading(true);
                setError(null);

                // Call the service to get backend configuration
                const result = await AuthService.GetServiceCallWithToken('Configuration/GetConfiguration');
                console.log(result);
                // Ensure that the component is still mounted
                if (isMounted) {
                    // Check for AuthService handled errors
                    if (result?.success === false) {
                        throw new Error(result.message || 'Failed to fetch configuration');
                    }

                    // Parse the API result structure (expecting { data: { data: { ... } } })
                    if (result?.responseCode === 200 && result?.responseStatus === 'success' && result?.data?.data) {
                        setConfiguration(result.data.data);
                    } else {
                        console.warn('Unexpected API response structure:', result);
                    }
                }
            } catch (err) {
                if (isMounted) {
                    setError(err.message || 'Failed to fetch configuration');
                    console.error('[ConfigurationProvider] API error:', err);
                }
            } finally {
                if (isMounted) {
                    setLoading(false);
                }
            }
        };

        fetchConfig();

        return () => {
            isMounted = false;
        };
    }, [user]); // Re-run whenever the user's authentication state changes

    // Exposed value for consumers
    const value = {
        configuration,
        loading,
        error,
    };

    return (
        <ConfigurationContext.Provider value={value}>
            {children}
        </ConfigurationContext.Provider>
    );
};
