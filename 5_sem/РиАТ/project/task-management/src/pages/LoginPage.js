import React, { useState } from 'react';
import AuthForm from '../components/AuthForm';
import {useAuth} from "../providers/AuthProvider";
import {hashFunction} from "../utils/hashFunction";

const LoginPage = () => {
    const [formData, setFormData] = useState({ email: '', password: '' });
    const [errorMessage, setErrorMessage] = useState('');
    const { saveToken } = useAuth();

    const handleInputChange = (e) => {
        const { id, value } = e.target;
        setFormData((prevData) => ({ ...prevData, [id]: value }));
    };

    const handleLogin = async (e) => {
        e.preventDefault();
        setErrorMessage('');

        try {
            const dto = {...formData, 'password': hashFunction(formData.password)}
            const response = await fetch('http://localhost:3010/api/auth/signin', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(dto),
            });

            if (!response.ok) {
                const errorData = await response.text();
                setErrorMessage(errorData || 'Login Error!');
                return;
            }

            const data = await response.json();
            saveToken(data.token);
            alert('Login Successful!');
        } catch (error) {
            setErrorMessage('Server Connection Error!');
        }
    };

    return (
        <div>
            <AuthForm
                title="Login"
                buttonText="Sign In"
                fields={[
                    { id: 'email', label: 'Email', type: 'email', value: formData.email },
                    { id: 'password', label: 'Password', type: 'password', value: formData.password },
                ]}
                onSubmit={handleLogin}
                onInputChange={handleInputChange}
            />
            {errorMessage && <div className="alert alert-danger mt-3">{errorMessage}</div>}
        </div>
    );
};

export default LoginPage;
