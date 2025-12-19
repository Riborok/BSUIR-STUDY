import { createContext, useContext, useState, useEffect, ReactNode } from "react"
import { Project, TaskItem, Attachment } from "../utils/tasks"
import { projectsApi, tasksApi } from "../api/client"

interface AppContextType {
    projects: Project[]
    tasks: TaskItem[]
    loading: boolean
    error: string | null
    addProject: (name: string, description?: string, users?: string[]) => Promise<void>
    updateProject: (id: string, name: string, description?: string) => Promise<void>
    deleteProject: (id: string) => Promise<void>
    addTask: (task: { title: string; description: string; user: string; status: string; projectId: string; attachments?: Attachment[] }) => Promise<void>
    updateTask: (taskId: string, updates: { title?: string; description?: string; user?: string; status?: string; projectId?: string; attachments?: Attachment[] }) => Promise<void>
    deleteTask: (taskId: string) => Promise<void>
    refreshProjects: () => Promise<void>
    refreshTasks: () => Promise<void>
}

const AppContext = createContext<AppContextType | undefined>(undefined)

export function AppProvider({ children }: { children: ReactNode }) {
    const [projects, setProjects] = useState<Project[]>([])
    const [tasks, setTasks] = useState<TaskItem[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)

    const refreshProjects = async () => {
        try {
            setError(null)
            const data = await projectsApi.getAll()
            setProjects(data)
        } catch (err) {
            setError(err instanceof Error ? err.message : "Failed to load projects")
            console.error("Error loading projects:", err)
        }
    }

    const refreshTasks = async () => {
        try {
            setError(null)
            const data = await tasksApi.getAll()
            setTasks(data)
        } catch (err) {
            setError(err instanceof Error ? err.message : "Failed to load tasks")
            console.error("Error loading tasks:", err)
        }
    }

    useEffect(() => {
        const loadInitialData = async () => {
            setLoading(true)
            await Promise.all([refreshProjects(), refreshTasks()])
            setLoading(false)
        }
        loadInitialData()
    }, [])

    const addProject = async (name: string, description?: string, users?: string[]) => {
        try {
            setError(null)
            const newProject = await projectsApi.create(name, description, users)
            setProjects((prev) => [...prev, newProject])
        } catch (err) {
            setError(err instanceof Error ? err.message : "Failed to add project")
            throw err
        }
    }

    const updateProject = async (id: string, name: string, description?: string) => {
        try {
            setError(null)
            const updatedProject = await projectsApi.update(id, name, description)
            setProjects((prev) =>
                prev.map((project) => (project.id === id ? updatedProject : project))
            )
        } catch (err) {
            setError(err instanceof Error ? err.message : "Failed to update project")
            throw err
        }
    }

    const deleteProject = async (id: string) => {
        try {
            setError(null)
            await projectsApi.delete(id)
            setProjects((prev) => prev.filter((project) => project.id !== id))
            setTasks((prev) => prev.filter((task) => task.projectId !== id))
        } catch (err) {
            setError(err instanceof Error ? err.message : "Failed to delete project")
            throw err
        }
    }

    const addTask = async (task: { title: string; description: string; user: string; status: string; projectId: string; attachments?: Attachment[] }) => {
        try {
            setError(null)
            const newTask = await tasksApi.create(task)
            setTasks((prev) => [...prev, newTask])
        } catch (err) {
            setError(err instanceof Error ? err.message : "Failed to add task")
            throw err
        }
    }

    const updateTask = async (taskId: string, updates: { title?: string; description?: string; user?: string; status?: string; projectId?: string; attachments?: Attachment[] }) => {
        try {
            setError(null)
            const updatedTask = await tasksApi.update(taskId, updates)
            setTasks((prev) =>
                prev.map((task) => (task.id === taskId ? updatedTask : task))
            )
        } catch (err) {
            setError(err instanceof Error ? err.message : "Failed to update task")
            throw err
        }
    }

    const deleteTask = async (taskId: string) => {
        try {
            setError(null)
            await tasksApi.delete(taskId)
            setTasks((prev) => prev.filter((task) => task.id !== taskId))
        } catch (err) {
            setError(err instanceof Error ? err.message : "Failed to delete task")
            throw err
        }
    }

    return (
        <AppContext.Provider
            value={{
                projects,
                tasks,
                loading,
                error,
                addProject,
                updateProject,
                deleteProject,
                addTask,
                updateTask,
                deleteTask,
                refreshProjects,
                refreshTasks,
            }}
        >
            {children}
        </AppContext.Provider>
    )
}

export function useApp() {
    const context = useContext(AppContext)
    if (context === undefined) {
        throw new Error("useApp must be used within an AppProvider")
    }
    return context
}
