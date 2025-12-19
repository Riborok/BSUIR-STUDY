import { Link, useLocation, useNavigate } from "react-router-dom"
import { authApi, tokenStorage } from "../api/client"

export default function Nav() {
    const location = useLocation()
    const navigate = useNavigate()
    const user = tokenStorage.getUser()

    const handleLogout = () => {
        authApi.logout()
        navigate('/')
        window.location.reload()
    }

    const navItems = [
        { path: "/", label: "Главная" },
        { path: "/projects", label: "Проекты" },
        { path: "/profile", label: "Профиль" },
    ]

    return (
        <nav className="mb-8">
            <div className="flex items-center justify-between">
                <div className="bg-gray-800 border border-gray-700 rounded-lg p-1 inline-flex gap-1">
                    {navItems.map((item) => {
                        const isActive =
                            location.pathname === item.path ||
                            (item.path === "/projects" && location.pathname.startsWith("/projects"))

                        return (
                            <Link
                                key={item.path}
                                to={item.path}
                                className={`
                                    px-5 py-2 rounded text-sm font-medium transition-colors
                                    ${
                                        isActive
                                            ? "bg-violet-600 text-white"
                                            : "text-gray-400 hover:text-white hover:bg-gray-700"
                                    }
                                `}
                            >
                                {item.label}
                            </Link>
                        )
                    })}
                </div>

                {user && (
                    <div className="flex items-center gap-4">
                        <div className="text-sm">
                            <span className="text-gray-400">Пользователь: </span>
                            <span className="text-white font-medium">{user.login}</span>
                            <span className={`ml-2 px-2 py-0.5 rounded text-xs ${
                                user.role === 'admin' 
                                    ? 'bg-red-600 text-white' 
                                    : 'bg-blue-600 text-white'
                            }`}>
                                {user.role === 'admin' ? 'Администратор' : 'Участник'}
                            </span>
                        </div>
                        <button
                            onClick={handleLogout}
                            className="px-4 py-2 bg-red-600 hover:bg-red-700 text-white text-sm font-medium rounded transition-colors"
                        >
                            Выход
                        </button>
                    </div>
                )}
            </div>
        </nav>
    )
}
