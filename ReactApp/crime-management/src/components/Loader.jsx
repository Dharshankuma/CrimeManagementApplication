import React from 'react';
import { useLoader } from '../context/LoaderContext';

const Loader = () => {
    const { loading } = useLoader();

    if (!loading) return null;

    return (
        <div style={styles.overlay}>
            <div className="spinner-border text-light" role="status" style={styles.spinner}>
                <span className="visually-hidden">Loading...</span>
            </div>
        </div>
    );
};

const styles = {
    overlay: {
        position: 'fixed',
        top: 0,
        left: 0,
        width: '100vw',
        height: '100vh',
        backgroundColor: 'rgba(0, 0, 0, 0.6)',
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        zIndex: 9999, // Ensure it sits exactly on top of everything
    },
    spinner: {
        width: '4rem',
        height: '4rem',
        borderWidth: '0.4em'
    }
};

export default Loader;
