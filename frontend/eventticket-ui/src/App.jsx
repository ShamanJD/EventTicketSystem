import { Routes, Route, Link, useLocation } from 'react-router-dom';
import BookingPage from './features/bookings/BookingPage';
import ConcertsPage from './features/concerts/ConcertsPage';
import Login from './Login';
import { useState } from 'react';
import './App.css';

function App() {
    const [isAuthenticated, setIsAuthenticated] = useState(!!localStorage.getItem('accessToken'));
    const location = useLocation();

    const handleLogout = () => {
        localStorage.clear();
        setIsAuthenticated(false);
    };

    if (!isAuthenticated) {
        return <Login onLoginSuccess={() => setIsAuthenticated(true)} />;
    }

    const isActive = (path) => location.pathname === path;

    return (
        <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
            <header className="glass" style={{
                padding: '1rem 2rem',
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center',
                borderBottom: '1px solid var(--border-color)',
                position: 'sticky',
                top: 0,
                zIndex: 100,
                backdropFilter: 'blur(20px)'
            }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: '2rem' }}>
                    <div style={{
                        display: 'flex',
                        alignItems: 'center',
                        gap: '0.75rem'
                    }}>
                        <div style={{
                            width: '40px',
                            height: '40px',
                            background: 'var(--primary-gradient)',
                            borderRadius: '10px',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            fontSize: '1.25rem'
                        }}>
                            🎫
                        </div>
                        <h1 style={{
                            fontSize: '1.25rem',
                            fontWeight: '700',
                            margin: 0
                        }}>
                            EventTicket
                        </h1>
                    </div>

                    <nav style={{ display: 'flex', gap: '0.5rem' }}>
                        <Link
                            to="/"
                            style={{
                                padding: '0.625rem 1.25rem',
                                borderRadius: '10px',
                                fontWeight: '600',
                                fontSize: '0.95rem',
                                transition: 'all 0.3s ease',
                                background: isActive('/') ? 'var(--bg-card-hover)' : 'transparent',
                                color: isActive('/') ? 'var(--primary-500)' : 'var(--text-secondary)'
                            }}
                        >
                            Концерты
                        </Link>
                        <Link
                            to="/booking"
                            style={{
                                padding: '0.625rem 1.25rem',
                                borderRadius: '10px',
                                fontWeight: '600',
                                fontSize: '0.95rem',
                                transition: 'all 0.3s ease',
                                background: isActive('/booking') ? 'var(--bg-card-hover)' : 'transparent',
                                color: isActive('/booking') ? 'var(--primary-500)' : 'var(--text-secondary)'
                            }}
                        >
                            Бронирование
                        </Link>
                    </nav>
                </div>

                <button
                    onClick={handleLogout}
                    style={{
                        background: 'rgba(245, 87, 108, 0.15)',
                        color: '#f5576c',
                        padding: '0.625rem 1.25rem',
                        borderRadius: '10px',
                        fontWeight: '600',
                        fontSize: '0.875rem',
                        border: '1px solid rgba(245, 87, 108, 0.3)',
                        transition: 'all 0.3s ease'
                    }}
                    onMouseEnter={(e) => {
                        e.target.style.background = 'rgba(245, 87, 108, 0.25)';
                        e.target.style.transform = 'translateY(-1px)';
                    }}
                    onMouseLeave={(e) => {
                        e.target.style.background = 'rgba(245, 87, 108, 0.15)';
                        e.target.style.transform = 'translateY(0)';
                    }}
                >
                    Выйти
                </button>
            </header>

            <main style={{ flex: 1 }}>
                <Routes>
                    <Route path="/" element={<ConcertsPage />} />
                    <Route path="/booking" element={<BookingPage />} />
                </Routes>
            </main>

            <footer style={{
                padding: '1.5rem 2rem',
                textAlign: 'center',
                borderTop: '1px solid var(--border-color)',
                color: 'var(--text-muted)',
                fontSize: '0.875rem'
            }}>
                <p>© 2025 EventTicket System. Все права защищены.</p>
            </footer>
        </div>
    );
}

export default App;
