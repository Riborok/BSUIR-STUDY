import { useState } from "react"
import { useNavigate, useParams } from "react-router-dom"
import { useApp } from "../context/AppContext"
import ProjectList from "../components/ProjectList"
import ProjectDetail from "../components/ProjectDetail.tsx"
import ProjectForm from "../components/ProjectForm"

export default function Projects() {
    const { id } = useParams<{ id: string }>()
    const navigate = useNavigate()
    const { projects, tasks, addProject, loading } = useApp()
    const [showProjectForm, setShowProjectForm] = useState(false)
    const [error, setError] = useState<string>('')

    const selectedProject = id ? projects.find((p) => p.id === id) : null

    const handleCreateProject = async (name: string, description: string, users: string[]) => {
        try {
            setError('')
            await addProject(name, description, users)
            setShowProjectForm(false)
        } catch (error: any) {
            console.error("Failed to create project:", error)
            setError(error.message || "Не удалось создать проект")
        }
    }

    const handleProjectClick = (projectId: string) => {
        navigate(`/projects/${projectId}`)
    }

    const handleBack = () => {
        navigate("/projects")
    }

    if (loading) {
        return (
            <section className="container mx-auto px-4">
                <div className="text-center py-20 text-white text-xl">
                    Загрузка...
                </div>
            </section>
        )
    }

    return (
        <section className="container mx-auto px-4">
            {selectedProject ? (
                <ProjectDetail project={selectedProject} onBack={handleBack} />
            ) : (
                <div>
                    {error && (
                        <div className="mb-4 bg-red-900 border border-red-700 text-red-200 px-4 py-3 rounded">
                            <strong className="font-bold">Ошибка: </strong>
                            <span>{error}</span>
                        </div>
                    )}
                    <div className="mb-6 flex items-center justify-between">
                        <div>
                            <h2 className="text-3xl font-bold text-white mb-2">
                                Проекты
                            </h2>
                            <p className="text-gray-400">Управление проектами и задачами</p>
                        </div>
                        <button
                            onClick={() => setShowProjectForm(!showProjectForm)}
                            className="bg-violet-600 text-white px-5 py-2 rounded hover:bg-violet-700 transition-colors font-medium"
                        >
                            {showProjectForm ? "Отменить" : "Создать проект"}
                        </button>
                    </div>

                    {showProjectForm && (
                        <ProjectForm
                            onSubmit={handleCreateProject}
                            onCancel={() => setShowProjectForm(false)}
                        />
                    )}

                    <ProjectList
                        projects={projects}
                        tasks={tasks}
                        onProjectClick={handleProjectClick}
                    />
                </div>
            )}
        </section>
    )
}
