import { useState } from 'react';

function Login({ onLoginSuccess }) {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setIsLoading(true);

        try {
            const response = await fetch('https://localhost:7000/api/auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username, password }),
            });

            if (response.ok) {
                const data = await response.json();
                localStorage.setItem('accessToken', data.accessToken);
                localStorage.setItem('refreshToken', data.refreshToken);
                onLoginSuccess();
            } else {
                setError('Неверный логин или пароль');
            }
        } catch (err) {
            setError('Ошибка подключения к серверу');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div style={{
            minHeight: '100vh',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            background: 'linear-gradient(135deg, #667eea 0%, #764ba2 50%, #f093fb 100%)',
            backgroundSize: '400% 400%',
            animation: 'gradientShift 15s ease infinite',
            padding: '1rem'
        }}>
            <style>{`
                @keyframes gradientShift {
                    0% { background-position: 0% 50%; }
                    50% { background-position: 100% 50%; }
                    100% { background-position: 0% 50%; }
                }
            `}</style>

            <div className="glass animate-fade-in" style={{
                maxWidth: '420px',
                width: '100%',
                borderRadius: '24px',
                padding: '2.5rem',
                boxShadow: '0 8px 32px rgba(0, 0, 0, 0.3)',
            }}>
                <div style={{ textAlign: 'center', marginBottom: '2rem' }}>
                    <div style={{
                        width: '80px',
                        height: '80px',
                        margin: '0 auto 1.5rem',
                        background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                        borderRadius: '20px',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        fontSize: '2.5rem',
                        boxShadow: '0 8px 24px rgba(102, 126, 234, 0.4)'
                    }}>
                        🎫
                    </div>
                    <h2 style={{
                        marginBottom: '0.5rem',
                        fontSize: '1.875rem',
                        fontWeight: '700'
                    }}>
                        EventTicket
                    </h2>
                    <p style={{
                        color: 'var(--text-secondary)',
                        fontSize: '0.95rem'
                    }}>
                        Войдите в свой аккаунт
                    </p>
                </div>

                <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
                    <div>
                        <label style={{
                            display: 'block',
                            marginBottom: '0.5rem',
                            fontSize: '0.875rem',
                            fontWeight: '600',
                            color: 'var(--text-secondary)'
                        }}>
                            Логин
                        </label>
                        <input
                            type="text"
                            placeholder="Введите логин"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            required
                            style={{
                                fontSize: '1rem',
                            }}
                        />
                    </div>

                    <div>
                        <label style={{
                            display: 'block',
                            marginBottom: '0.5rem',
                            fontSize: '0.875rem',
                            fontWeight: '600',
                            color: 'var(--text-secondary)'
                        }}>
                            Пароль
                        </label>
                        <input
                            type="password"
                            placeholder="Введите пароль"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                            style={{
                                fontSize: '1rem',
                            }}
                        />
                    </div>

                    {error && (
                        <div className="animate-fade-in" style={{
                            padding: '0.875rem',
                            background: 'rgba(245, 87, 108, 0.1)',
                            border: '1px solid rgba(245, 87, 108, 0.3)',
                            borderRadius: '12px',
                            color: '#f5576c',
                            fontSize: '0.875rem',
                            fontWeight: '500'
                        }}>
                            ⚠️ {error}
                        </div>
                    )}

                    <button
                        type="submit"
                        disabled={isLoading}
                        style={{
                            marginTop: '0.5rem',
                            background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                            color: 'white',
                            padding: '1rem',
                            fontSize: '1rem',
                            fontWeight: '600',
                            borderRadius: '12px',
                            cursor: isLoading ? 'not-allowed' : 'pointer',
                            transition: 'all 0.3s ease',
                            boxShadow: '0 4px 16px rgba(102, 126, 234, 0.4)',
                            transform: isLoading ? 'scale(0.98)' : 'scale(1)',
                        }}
                        onMouseEnter={(e) => {
                            if (!isLoading) {
                                e.target.style.transform = 'translateY(-2px)';
                                e.target.style.boxShadow = '0 8px 24px rgba(102, 126, 234, 0.5)';
                            }
                        }}
                        onMouseLeave={(e) => {
                            if (!isLoading) {
                                e.target.style.transform = 'translateY(0)';
                                e.target.style.boxShadow = '0 4px 16px rgba(102, 126, 234, 0.4)';
                            }
                        }}
                    >
                        {isLoading ? (
                            <span style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '0.5rem' }}>
                                <span className="animate-pulse">⏳</span> Вход...
                            </span>
                        ) : (
                            'Войти'
                        )}
                    </button>
                </form>

                <div style={{
                    marginTop: '1.5rem',
                    paddingTop: '1.5rem',
                    borderTop: '1px solid var(--border-color)',
                    textAlign: 'center',
                    fontSize: '0.875rem',
                    color: 'var(--text-muted)'
                }}>
                    Нет аккаунта? <a href="#" style={{ color: 'var(--primary-500)', fontWeight: '600' }}>Зарегистрироваться</a>
                </div>
            </div>
        </div>
    );
}

export default Login;
