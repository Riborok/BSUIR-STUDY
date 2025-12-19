import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom"
import { useState } from "react"
import "./App.css"
import Nav from "./components/Nav"
import Home from "./pages/Home"
import Projects from "./pages/Projects"
import Profile from "./pages/Profile"
import { AppProvider } from "./context/AppContext"
import { LoginForm } from "./components/LoginForm"
import { RegisterForm } from "./components/RegisterForm"
import { tokenStorage } from "./api/client"

function App() {
    const [isAuthenticated, setIsAuthenticated] = useState(() => {
        return !!tokenStorage.getAccessToken()
    })
    const [showRegister, setShowRegister] = useState(false)

    const handleAuthSuccess = () => {
        setIsAuthenticated(true)
        setShowRegister(false)
    }

    if (!isAuthenticated) {
        return (
            <div className="min-h-screen bg-gray-900">
                {showRegister ? (
                    <RegisterForm
                        onSuccess={() => setShowRegister(false)}
                        onSwitchToLogin={() => setShowRegister(false)}
                    />
                ) : (
                    <LoginForm
                        onSuccess={handleAuthSuccess}
                        onSwitchToRegister={() => setShowRegister(true)}
                    />
                )}
            </div>
        )
    }

    return (
        <AppProvider>
            <Router>
                <div>
                    <Nav />
                    <Routes>
                        <Route path="/" element={<Home />} />
                        <Route path="/projects" element={<Projects />} />
                        <Route path="/projects/:id" element={<Projects />} />
                        <Route path="/profile" element={<Profile />} />
                        <Route path="*" element={<Navigate to="/" replace />} />
                    </Routes>
                </div>
            </Router>
        </AppProvider>
    )
}

export default App
