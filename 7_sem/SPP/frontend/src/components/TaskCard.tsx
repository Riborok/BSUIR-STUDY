import { useState } from "react"
import { TaskItem, TaskStatus, getUserId, getUserName, Attachment } from "../utils/tasks"

interface TaskCardProps {
    task: TaskItem
    onUpdate?: (taskId: string, updates: { title?: string; description?: string; user?: string; status?: string; projectId?: string; attachments?: Attachment[] }) => void
    onDelete?: (taskId: string) => void
}

export default function TaskCard({ task, onUpdate, onDelete }: TaskCardProps) {
    const [isEditing, setIsEditing] = useState(false)
    const [title, setTitle] = useState(task.title)
    const [description, setDescription] = useState(task.description)
    const [userId, setUserId] = useState(getUserId(task.user))
    const [status, setStatus] = useState<TaskStatus>(task.status)

    const handleSave = () => {
        if (onUpdate && title.trim() && userId.trim()) {
            onUpdate(task.id, { title, description, user: userId, status })
            setIsEditing(false)
        }
    }

    const handleCancel = () => {
        setTitle(task.title)
        setDescription(task.description)
        setUserId(getUserId(task.user))
        setStatus(task.status)
        setIsEditing(false)
    }

    const handleDelete = () => {
        if (onDelete) {
            onDelete(task.id)
        }
    }

    const handleDownloadAttachment = (attachment: Attachment) => {
        try {
            const byteCharacters = atob(attachment.fileData)
            const byteNumbers = new Array(byteCharacters.length)
            for (let i = 0; i < byteCharacters.length; i++) {
                byteNumbers[i] = byteCharacters.charCodeAt(i)
            }
            const byteArray = new Uint8Array(byteNumbers)
            const blob = new Blob([byteArray], { type: attachment.mimeType })
            const url = URL.createObjectURL(blob)
            const link = document.createElement('a')
            link.href = url
            link.download = attachment.fileName
            document.body.appendChild(link)
            link.click()
            document.body.removeChild(link)
            URL.revokeObjectURL(url)
        } catch (error) {
            console.error('Ошибка при скачивании файла:', error)
        }
    }

    const formatFileSize = (bytes: number): string => {
        if (bytes < 1024) return bytes + ' B'
        if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB'
        return (bytes / (1024 * 1024)).toFixed(1) + ' MB'
    }

    const getFileIcon = (mimeType: string): string => {
        if (mimeType.startsWith('image/')) return '🖼️'
        if (mimeType.startsWith('video/')) return '🎥'
        if (mimeType.startsWith('audio/')) return '🎵'
        if (mimeType.includes('pdf')) return '📄'
        if (mimeType.includes('zip') || mimeType.includes('rar')) return '📦'
        if (mimeType.includes('json') || mimeType.includes('xml')) return '📋'
        return '📎'
    }

    if (isEditing) {
        return (
            <div className="bg-gray-800 border-2 border-violet-600 rounded-lg p-4 mb-3">
                <div className="mb-3">
                    <label className="block text-xs font-medium text-gray-400 mb-1">Название</label>
                    <input
                        type="text"
                        value={title}
                        onChange={(e) => setTitle(e.target.value)}
                        className="w-full border border-gray-600 bg-gray-700 text-white rounded px-3 py-2 text-sm focus:border-violet-500 outline-none"
                        placeholder="Название задачи"
                    />
                </div>
                <div className="mb-3">
                    <label className="block text-xs font-medium text-gray-400 mb-1">Описание</label>
                    <textarea
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                        className="w-full border border-gray-600 bg-gray-700 text-white rounded px-3 py-2 text-sm focus:border-violet-500 outline-none resize-none"
                        rows={3}
                        placeholder="Описание задачи"
                    />
                </div>
                <div className="mb-3">
                    <label className="block text-xs font-medium text-gray-400 mb-1">ID Пользователя</label>
                    <input
                        type="text"
                        value={userId}
                        onChange={(e) => setUserId(e.target.value)}
                        className="w-full border border-gray-600 bg-gray-700 text-white rounded px-3 py-2 text-sm focus:border-violet-500 outline-none"
                        placeholder="ID пользователя"
                    />
                </div>
                <div className="mb-4">
                    <label className="block text-xs font-medium text-gray-400 mb-1">Статус</label>
                    <select
                        value={status}
                        onChange={(e) => setStatus(e.target.value as TaskStatus)}
                        className="w-full border border-gray-600 bg-gray-700 text-white rounded px-3 py-2 text-sm focus:border-violet-500 outline-none cursor-pointer"
                    >
                        <option value="todo">To Do</option>
                        <option value="in_progress">In Progress</option>
                        <option value="done">Done</option>
                    </select>
                </div>
                <div className="flex gap-2">
                    <button
                        onClick={handleSave}
                        className="flex-1 bg-violet-600 text-white px-4 py-2 rounded text-sm font-medium hover:bg-violet-700 transition-colors"
                    >
                        Сохранить
                    </button>
                    <button
                        onClick={handleCancel}
                        className="flex-1 bg-gray-700 text-white px-4 py-2 rounded text-sm font-medium hover:bg-gray-600 transition-colors"
                    >
                        Отмена
                    </button>
                </div>
            </div>
        )
    }

    const displayName = getUserName(task.user)

    return (
        <div className="group bg-gray-800 border border-gray-700 rounded-lg p-4 mb-3 hover:border-violet-500 transition-colors">
            <div className="flex items-start justify-between mb-2">
                <h4 className="font-semibold text-white text-sm flex-1 pr-2">{task.title}</h4>
                <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                    {onUpdate && (
                        <button
                            onClick={() => setIsEditing(true)}
                            className="text-violet-400 hover:text-violet-300 text-sm px-2 py-1 rounded hover:bg-gray-700 transition-colors"
                            title="Редактировать"
                        >
                            ✏️
                        </button>
                    )}
                    {onDelete && (
                        <button
                            onClick={handleDelete}
                            className="text-red-400 hover:text-red-300 text-sm px-2 py-1 rounded hover:bg-gray-700 transition-colors"
                            title="Удалить"
                        >
                            🗑️
                        </button>
                    )}
                </div>
            </div>
            {task.description && (
                <p className="text-xs text-gray-400 mb-3">{task.description}</p>
            )}

            {task.attachments && task.attachments.length > 0 && (
                <div className="mb-3 space-y-1">
                    <span className="text-xs text-gray-500 block mb-1">Вложения:</span>
                    {task.attachments.map((attachment, index) => (
                        <button
                            key={index}
                            onClick={() => handleDownloadAttachment(attachment)}
                            className="flex items-center gap-2 w-full text-left p-2 rounded bg-gray-700 hover:bg-gray-600 group/file"
                        >
                            <span className="text-lg">{getFileIcon(attachment.mimeType)}</span>
                            <div className="flex-1 min-w-0">
                                <div className="text-xs text-white truncate">
                                    {attachment.fileName}
                                </div>
                                <div className="text-xs text-gray-500">
                                    {formatFileSize(attachment.fileSize)}
                                </div>
                            </div>
                        </button>
                    ))}
                </div>
            )}

            <div className="flex items-center gap-2 pt-2 border-t border-gray-700">
                <span className="text-xs text-gray-500">Исполнитель:</span>
                <span className="text-xs font-medium text-violet-400">{displayName}</span>
            </div>
        </div>
    )
}
