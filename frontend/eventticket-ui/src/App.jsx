import { useEffect, useState } from 'react';
import { authorizedFetch } from './api';
import Login from './Login';
import './App.css';

function App() {
    const [concerts, setConcerts] = useState([]);
    const [loading, setLoading] = useState(false);
    // Проверяем, есть ли токен при загрузке
    const [isAuthenticated, setIsAuthenticated] = useState(!!localStorage.getItem('accessToken'));

    const fetchConcerts = async () => {
        setLoading(true);
        try {
            const response = await authorizedFetch('/concerts');
            if (response.ok) {
                const data = await response.json();
                setConcerts(data);
            } else if (response.status === 401) {
                setIsAuthenticated(false);
            }
        } catch (error) {
            console.error("Ошибка:", error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (isAuthenticated) {
            fetchConcerts();
        }
    }, [isAuthenticated]);

    const handleLogout = () => {
        localStorage.clear();
        setIsAuthenticated(false);
        setConcerts([]);
    };

    if (!isAuthenticated) {
        return <Login onLoginSuccess={() => setIsAuthenticated(true)} />;
    }

    return (
        <div className="App">
            <header className="App-header">
                <h1>EventTicket Dashboard</h1>
                <button onClick={handleLogout} style={{ marginBottom: '20px' }}>Выйти</button>

                {loading ? <p>Загрузка данных...</p> : (
                    <div className="concert-grid">
                        {concerts.length > 0 ? concerts.map(c => (
                            <div key={c.id} className="card">
                                <h3>{c.name}</h3>
                                <p>Дата: {c.date}</p>
                            </div>
                        )) : <p>Концертов пока нет</p>}
                    </div>
                )}
            </header>
        </div>
    );
}

export default App;