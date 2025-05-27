import React, { createContext, useContext, useState, useMemo } from 'react';

const AuthContext = createContext();

export const useAuth = () => useContext(AuthContext);

const AuthProvider = ({ children }) => {
    const [token, setToken] = useState(() => localStorage.getItem('authToken'));

    const saveToken = (newToken) => {
        setToken(newToken);
        localStorage.setItem('authToken', newToken);
    };

    const clearToken = () => {
        setToken(null);
        localStorage.removeItem('authToken');
    };

    const parseJwt = (token) => {
        try {
            const payloadBase64 = token.split('.')[1];
            const payloadDecoded = atob(payloadBase64);
            return JSON.parse(payloadDecoded);
        } catch (error) {
            return null;
        }
    };

    const parsedToken = useMemo(() => (token ? parseJwt(token) : null), [token]);

    return (
        <AuthContext.Provider value={{ token, saveToken, clearToken, parsedToken }}>
            {children}
        </AuthContext.Provider>
    );
};

export default AuthProvider;
