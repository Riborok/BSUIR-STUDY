import React, { useState } from 'react';
import AuthForm from '../components/AuthForm';
import {useAuth} from "../providers/AuthProvider";
import {hashFunction} from "../utils/hashFunction";

const RegisterPage = () => {
    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        email: '',
        password: '',
    });

    const [errorMessage, setErrorMessage] = useState('');
    const { saveToken } = useAuth();

    const handleInputChange = (e) => {
        const { id, value } = e.target;
        setFormData((prevData) => ({ ...prevData, [id]: value }));
    };

    const handleRegister = async (e) => {
        e.preventDefault();
        setErrorMessage('');

        try {
            const dto = {...formData, 'password': hashFunction(formData.password)}
            const response = await fetch('http://localhost:3010/api/auth/signup', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(dto),
            });

            if (!response.ok) {
                const errorData = await response.text();
                setErrorMessage(errorData || 'Registration Error!');
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
                title="Registration"
                buttonText="Sign Up"
                fields={[
                    { id: 'firstName', label: 'First Name', value: formData.firstName },
                    { id: 'lastName', label: 'Last Name', value: formData.lastName },
                    { id: 'email', label: 'Email', type: 'email', value: formData.email },
                    { id: 'password', label: 'Password', type: 'password', value: formData.password },
                ]}
                onSubmit={handleRegister}
                onInputChange={handleInputChange}
            />
            {errorMessage && <div className="alert alert-danger mt-3">{errorMessage}</div>}
        </div>
    );
};

export default RegisterPage;
