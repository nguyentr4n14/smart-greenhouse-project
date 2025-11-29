import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import Dashboard from './pages/Dashboard';
import LiveDashboardPage from './pages/LiveDashboardPage';

export default function App() {
    return (
        <BrowserRouter>
            <nav className="bg-white shadow-sm border-b">
                <div className="max-w-7xl mx-auto px-8 py-4">
                    <div className="flex gap-6">
                        <Link 
                            to="/" 
                            className="text-gray-700 hover:text-blue-600 font-medium transition-colors"
                        >
                            Dashboard
                        </Link>
                        <Link 
                            to="/live" 
                            className="text-gray-700 hover:text-blue-600 font-medium transition-colors"
                        >
                            Live Readings
                        </Link>
                    </div>
                </div>
            </nav>
            <Routes>
                <Route path="/" element={<Dashboard />} />
                <Route path="/live" element={<LiveDashboardPage />} />
            </Routes>
        </BrowserRouter>
    );
}
