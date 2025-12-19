import { Link } from "react-router-dom"
import { useApp } from "../context/AppContext"

export default function Home() {
    const { projects, tasks } = useApp()

    const stats = [
        { label: "Проектов", value: projects.length },
        { label: "Всего задач", value: tasks.length },
        { label: "В работе", value: tasks.filter(t => t.status === "in_progress").length },
        { label: "Завершено", value: tasks.filter(t => t.status === "done").length },
    ]

    return (
        <section className="container mx-auto px-4">
            <div className="text-center mb-12">
                <h1 className="text-4xl font-bold text-white mb-4">
                    Task Manager
                </h1>
                <p className="text-lg text-gray-400 max-w-2xl mx-auto">
                    Система управления проектами и задачами
                </p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-12">
                {stats.map((stat, index) => (
                    <div
                        key={index}
                        className="bg-gray-800 border border-gray-700 rounded-lg p-6 text-center"
                    >
                        <div className="text-3xl font-bold text-white mb-1">{stat.value}</div>
                        <div className="text-sm text-gray-400">{stat.label}</div>
                    </div>
                ))}
            </div>
        </section>
    )
}
