import { useState } from 'react';
import { authApi } from '../api/client';

interface RegisterFormProps {
    onSuccess: () => void;
    onSwitchToLogin: () => void;
}

export const RegisterForm = ({ onSuccess, onSwitchToLogin }: RegisterFormProps) => {
    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        if (password !== confirmPassword) {
            setError('Пароли не совпадают');
            setLoading(false);
            return;
        }

        try {
            await authApi.register(login, password);
            setSuccess(true);
            setTimeout(() => {
                onSwitchToLogin();
            }, 100);
        } catch (err: any) {
            setError(err.message || 'Ошибка регистрации');
        } finally {
            setLoading(false);
        }
    };

    if (success) {
        return (
            <div style={{
                maxWidth: '400px',
                margin: '50px auto',
                padding: '30px',
                border: '1px solid rgba(34, 197, 94, 0.4)',
                borderRadius: '8px',
                backgroundColor: 'rgba(30, 30, 46, 0.8)',
                textAlign: 'center',
                color: 'rgba(255, 255, 255, 0.95)'
            }}>
                <h2 style={{ color: '#86efac' }}>Регистрация успешна!</h2>
                <p style={{ color: 'rgba(255, 255, 255, 0.87)' }}>Перенаправление на страницу входа...</p>
            </div>
        );
    }

    return (
        <div style={{
            maxWidth: '400px',
            margin: '50px auto',
            padding: '30px',
            border: '1px solid rgba(124, 58, 237, 0.3)',
            borderRadius: '8px',
            backgroundColor: 'rgba(30, 30, 46, 0.8)',
            color: 'rgba(255, 255, 255, 0.95)'
        }}>
            <h2 style={{ textAlign: 'center', marginBottom: '20px', color: 'rgba(255, 255, 255, 0.95)' }}>Регистрация</h2>

            <form onSubmit={handleSubmit}>
                <div style={{ marginBottom: '15px' }}>
                    <label style={{ display: 'block', marginBottom: '5px', color: 'rgba(255, 255, 255, 0.87)' }}>
                        Логин:
                    </label>
                    <input
                        type="text"
                        value={login}
                        onChange={(e) => setLogin(e.target.value)}
                        required
                        style={{
                            width: '100%',
                            padding: '8px',
                            borderRadius: '4px',
                            border: '1px solid rgba(124, 58, 237, 0.4)',
                            backgroundColor: 'rgba(26, 26, 46, 0.6)',
                            color: 'rgba(255, 255, 255, 0.95)'
                        }}
                    />
                </div>

                <div style={{ marginBottom: '15px' }}>
                    <label style={{ display: 'block', marginBottom: '5px', color: 'rgba(255, 255, 255, 0.87)' }}>
                        Пароль:
                    </label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                        style={{
                            width: '100%',
                            padding: '8px',
                            borderRadius: '4px',
                            border: '1px solid rgba(124, 58, 237, 0.4)',
                            backgroundColor: 'rgba(26, 26, 46, 0.6)',
                            color: 'rgba(255, 255, 255, 0.95)'
                        }}
                    />
                </div>

                <div style={{ marginBottom: '15px' }}>
                    <label style={{ display: 'block', marginBottom: '5px', color: 'rgba(255, 255, 255, 0.87)' }}>
                        Подтверждение пароля:
                    </label>
                    <input
                        type="password"
                        value={confirmPassword}
                        onChange={(e) => setConfirmPassword(e.target.value)}
                        required
                        style={{
                            width: '100%',
                            padding: '8px',
                            borderRadius: '4px',
                            border: '1px solid rgba(124, 58, 237, 0.4)',
                            backgroundColor: 'rgba(26, 26, 46, 0.6)',
                            color: 'rgba(255, 255, 255, 0.95)'
                        }}
                    />
                </div>

                {error && (
                    <div style={{
                        padding: '10px',
                        marginBottom: '15px',
                        backgroundColor: 'rgba(239, 68, 68, 0.2)',
                        color: '#fca5a5',
                        borderRadius: '4px',
                        border: '1px solid rgba(239, 68, 68, 0.4)'
                    }}>
                        {error}
                    </div>
                )}

                <button
                    type="submit"
                    disabled={loading}
                    style={{
                        width: '100%',
                        padding: '10px',
                        backgroundColor: loading ? 'rgba(124, 58, 237, 0.5)' : 'rgba(124, 58, 237, 0.8)',
                        color: '#fff',
                        border: 'none',
                        borderRadius: '4px',
                        cursor: loading ? 'not-allowed' : 'pointer',
                        fontSize: '16px',
                        transition: 'background-color 0.3s'
                    }}
                >
                    {loading ? 'Регистрация...' : 'Зарегистрироваться'}
                </button>
            </form>

            <div style={{ marginTop: '15px', textAlign: 'center' }}>
                <span style={{ color: 'rgba(255, 255, 255, 0.87)' }}>Уже есть аккаунт? </span>
                <button
                    onClick={onSwitchToLogin}
                    style={{
                        background: 'none',
                        border: 'none',
                        color: 'rgba(167, 139, 250, 0.9)',
                        cursor: 'pointer',
                        textDecoration: 'underline'
                    }}
                >
                    Войти
                </button>
            </div>
        </div>
    );
};
