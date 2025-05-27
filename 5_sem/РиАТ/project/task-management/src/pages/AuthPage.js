import React, { useState } from 'react';
import LoginPage from './LoginPage';
import RegisterPage from './RegisterPage';

const AuthPage = () => {
    const [isLogin, setIsLogin] = useState(true);

    return (
        <div className="container d-flex justify-content-center align-items-center vh-100">
            <div className="card shadow-lg" style={{ width: '50%', height: '100%' }}>
                <div className="card-body p-4 d-flex flex-column justify-content-center align-items-center">
                    <div className="card shadow-lg p-4" style={{ width: '80%' }}>
                        <div className="d-flex justify-content-between w-100 mb-4">
                            <button
                                className={`btn ${isLogin ? 'btn-primary' : 'btn-outline-primary'}`}
                                onClick={() => setIsLogin(true)}
                                style={{ width: '48%' }}
                            >
                                Login
                            </button>
                            <button
                                className={`btn ${!isLogin ? 'btn-primary' : 'btn-outline-primary'}`}
                                onClick={() => setIsLogin(false)}
                                style={{ width: '48%' }}
                            >
                                Sign Up
                            </button>
                        </div>
                        {isLogin ? <LoginPage /> : <RegisterPage />}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default AuthPage;
