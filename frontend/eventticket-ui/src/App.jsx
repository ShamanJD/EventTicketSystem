import { Routes, Route, Link } from 'react-router-dom';
import BookingPage from './features/bookings/BookingPage';
import ConcertsList from './ConcertsList';
import Login from './Login';
import { useState } from 'react';
import './App.css';

function App() {
    const [isAuthenticated, setIsAuthenticated] = useState(!!localStorage.getItem('accessToken'));

    const handleLogout = () => {
        localStorage.clear();
        setIsAuthenticated(false);
    };

    if (!isAuthenticated) {
        return <Login onLoginSuccess={() => setIsAuthenticated(true)} />;
    }

    return (
        <div className="App">
            <header className="p-4 bg-gray-800 text-white flex justify-between items-center">
                <nav className="flex space-x-4">
                    <Link to="/" className="text-blue-300 hover:text-white">Концерты</Link>
                    <Link to="/booking" className="text-blue-300 hover:text-white">Бронирование</Link>
                </nav>
                <div className="flex items-center space-x-4">
                    <h1>EventTicket Dashboard</h1>
                    <button onClick={handleLogout} className="bg-red-600 px-3 py-1 rounded">Выйти</button>
                </div>
            </header>

            <main className="p-4">
                <Routes>
                    <Route path="/" element={<ConcertsList />} />
                    <Route path="/booking" element={<BookingPage />} />
                </Routes>
            </main>
        </div>
    );
}

export default App;