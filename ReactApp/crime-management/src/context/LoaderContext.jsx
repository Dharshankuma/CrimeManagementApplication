import React, { createContext, useState, useContext, useEffect } from 'react';
import { setLoaderCallback } from '../services/AuthService';

const LoaderContext = createContext();

export const useLoader = () => {
    return useContext(LoaderContext);
};

export const LoaderProvider = ({ children }) => {
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        // Connect the setup from AuthService to our context's setLoading
        setLoaderCallback(setLoading);
    }, []);

    return (
        <LoaderContext.Provider value={{ loading, setLoading }}>
            {children}
        </LoaderContext.Provider>
    );
};
