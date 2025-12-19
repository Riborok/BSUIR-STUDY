import { useState } from 'react';
import { authApi } from '../api/client';

interface LoginFormProps {
    onSuccess: () => void;
    onSwitchToRegister: () => void;
}

export const LoginForm = ({ onSuccess, onSwitchToRegister }: LoginFormProps) => {
    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            await authApi.login(login, password);
            onSuccess();
        } catch (err: any) {
            setError(err.message || 'Ошибка входа');
        } finally {
            setLoading(false);
        }
    };

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
            <h2 style={{ textAlign: 'center', marginBottom: '20px', color: 'rgba(255, 255, 255, 0.95)' }}>Вход в систему</h2>

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

                <div style={{ marginBottom: '20px' }}>
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

                {error && (
                    <div style={{
                        padding: '10px',
                        marginBottom: '15px',
                        backgroundColor: 'rgba(239, 68, 68, 0.2)',
                        color: '#fca5a5',
                        borderRadius: '4px',
                        border: '1px solid rgba(239, 68, 68, 0.4)',
                        fontSize: '14px'
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
                        opacity: loading ? 0.6 : 1,
                        transition: 'background-color 0.3s'
                    }}
                >
                    {loading ? 'Вход...' : 'Войти'}
                </button>
            </form>

            <div style={{ marginTop: '20px', textAlign: 'center' }}>
                <button
                    onClick={onSwitchToRegister}
                    style={{
                        background: 'none',
                        border: 'none',
                        color: 'rgba(167, 139, 250, 0.9)',
                        cursor: 'pointer',
                        textDecoration: 'underline'
                    }}
                >
                    Нет аккаунта? Зарегистрироваться
                </button>
            </div>
        </div>
    );
};
