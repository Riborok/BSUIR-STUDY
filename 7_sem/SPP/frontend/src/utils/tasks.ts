export type TaskStatus = "todo" | "in_progress" | "done"

export interface Attachment {
    fileName: string
    fileData: string
    mimeType: string
    fileSize: number
}

export interface User {
    id: string
    login: string
    role: string
    projects?: string[]
    tasks?: string[]
}

export interface TaskItem {
    id: string
    title: string
    description: string
    user: User | null
    status: TaskStatus
    projectId: string
    attachments?: Attachment[]
}

export interface Project {
    id: string
    name: string
    description?: string
    users?: User[]
}

export function filterTasksByStatus(tasks: TaskItem[], status: TaskStatus): TaskItem[] {
    return tasks.filter((t) => t.status === status)
}

export function getTaskCountByProject(tasks: TaskItem[], projectId: string): number {
    return tasks.filter((t) => t.projectId === projectId).length
}

export function getUserId(user: User | null): string {
    return user?.id || ''
}

export function getUserName(user: User | null): string {
    return user?.login || user?.id || 'Не назначен'
}
