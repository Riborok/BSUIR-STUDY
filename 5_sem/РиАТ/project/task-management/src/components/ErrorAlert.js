import React from 'react';

const ErrorAlert = ({ message, onClose }) => {
    if (!message) return null;

    return (
        <div className="alert alert-danger alert-dismissible fade show" role="alert">
            {message}
            <button type="button" className="btn-close" aria-label="Close" onClick={onClose}></button>
        </div>
    );
};
export default ErrorAlert;
