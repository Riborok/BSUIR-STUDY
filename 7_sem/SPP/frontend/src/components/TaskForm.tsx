import { useState, FormEvent, ChangeEvent } from "react"
import { TaskStatus, Attachment } from "../utils/tasks"

interface TaskFormProps {
    onSubmit: (title: string, description: string, user: string, status: TaskStatus, attachments: Attachment[]) => void
    onCancel: () => void
}

export default function TaskForm({ onSubmit, onCancel }: TaskFormProps) {
    const [title, setTitle] = useState("")
    const [description, setDescription] = useState("")
    const [user, setUser] = useState("")
    const [status, setStatus] = useState<TaskStatus>("todo")
    const [attachments, setAttachments] = useState<Attachment[]>([])
    const [isUploading, setIsUploading] = useState(false)

    const handleFileChange = async (e: ChangeEvent<HTMLInputElement>) => {
        const files = e.target.files
        if (!files || files.length === 0) return

        setIsUploading(true)
        const newAttachments: Attachment[] = []

        for (let i = 0; i < files.length; i++) {
            const file = files[i]
            try {
                const base64 = await fileToBase64(file)
                newAttachments.push({
                    fileName: file.name,
                    fileData: base64,
                    mimeType: file.type || 'application/octet-stream',
                    fileSize: file.size
                })
            } catch (error) {
                console.error('Ошибка при чтении файла:', file.name, error)
            }
        }

        setAttachments([...attachments, ...newAttachments])
        setIsUploading(false)
        e.target.value = ''
    }

    const fileToBase64 = (file: File): Promise<string> => {
        return new Promise((resolve, reject) => {
            const reader = new FileReader()
            reader.onload = () => {
                const result = reader.result as string
                const base64 = result.split(',')[1]
                resolve(base64)
            }
            reader.onerror = reject
            reader.readAsDataURL(file)
        })
    }

    const removeAttachment = (index: number) => {
        setAttachments(attachments.filter((_, i) => i !== index))
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
        if (mimeType.includes('word') || mimeType.includes('document')) return '📝'
        if (mimeType.includes('sheet') || mimeType.includes('excel')) return '📊'
        if (mimeType.includes('presentation') || mimeType.includes('powerpoint')) return '📽️'
        if (mimeType.includes('json') || mimeType.includes('xml')) return '📋'
        return '📎'
    }

    const handleSubmit = (e: FormEvent) => {
        e.preventDefault()
        if (title.trim() && user.trim()) {
            onSubmit(title, description, user, status, attachments)
            setTitle("")
            setDescription("")
            setUser("")
            setStatus("todo")
            setAttachments([])
        }
    }

    return (
        <form
            onSubmit={handleSubmit}
            className="bg-gray-800 border border-gray-700 rounded-lg p-6"
        >
            <h3 className="text-lg font-semibold text-white mb-4">Создать задачу</h3>

            <div className="space-y-4">
                <div>
                    <label className="block text-sm font-medium text-gray-400 mb-1">
                        Название*
                    </label>
                    <input
                        type="text"
                        value={title}
                        onChange={(e) => setTitle(e.target.value)}
                        className="w-full border border-gray-600 bg-gray-700 text-white rounded px-3 py-2 focus:border-violet-500 outline-none"
                        placeholder="Введите название задачи"
                        required
                    />
                </div>

                <div>
                    <label className="block text-sm font-medium text-gray-400 mb-1">
                        Описание
                    </label>
                    <textarea
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                        className="w-full border border-gray-600 bg-gray-700 text-white rounded px-3 py-2 focus:border-violet-500 outline-none resize-none"
                        rows={4}
                        placeholder="Опишите задачу..."
                    />
                </div>

                <div>
                    <label className="block text-sm font-medium text-gray-400 mb-1">
                        Логин пользователя*
                    </label>
                    <input
                        type="text"
                        value={user}
                        onChange={(e) => setUser(e.target.value)}
                        className="w-full border border-gray-600 bg-gray-700 text-white rounded px-3 py-2 focus:border-violet-500 outline-none"
                        placeholder="Введите логин пользователя (например: ivan)"
                        required
                    />
                </div>

                <div>
                    <label className="block text-sm font-medium text-gray-400 mb-1">
                        Статус
                    </label>
                    <select
                        value={status}
                        onChange={(e) => setStatus(e.target.value as TaskStatus)}
                        className="w-full border border-gray-600 bg-gray-700 text-white rounded px-3 py-2 focus:border-violet-500 outline-none cursor-pointer"
                    >
                        <option value="todo">To Do</option>
                        <option value="in_progress">In Progress</option>
                        <option value="done">Done</option>
                    </select>
                </div>

                <div>
                    <label className="block text-sm font-medium text-gray-400 mb-1">
                        Вложения
                    </label>
                    <div className="space-y-2">
                        <label className="flex items-center justify-center w-full border-2 border-dashed border-gray-600 rounded-lg px-4 py-3 cursor-pointer hover:border-violet-500 transition-colors">
                            <span className="text-sm text-gray-400">
                                {isUploading ? '⏳ Загрузка...' : '📎 Нажмите для выбора файлов'}
                            </span>
                            <input
                                type="file"
                                multiple
                                onChange={handleFileChange}
                                className="hidden"
                                disabled={isUploading}
                            />
                        </label>

                        {attachments.length > 0 && (
                            <div className="space-y-1">
                                {attachments.map((attachment, index) => (
                                    <div
                                        key={index}
                                        className="flex items-center gap-2 p-2 rounded bg-gray-700 group"
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
                                        <button
                                            type="button"
                                            onClick={() => removeAttachment(index)}
                                            className="text-red-400 hover:text-red-300 opacity-0 group-hover:opacity-100 transition-opacity px-2"
                                        >
                                            ✕
                                        </button>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>
                </div>
            </div>

            <div className="flex gap-3 mt-6">
                <button
                    type="submit"
                    className="flex-1 bg-violet-600 text-white px-5 py-2 rounded hover:bg-violet-700 transition-colors font-medium"
                    disabled={isUploading}
                >
                    Создать
                </button>
                <button
                    type="button"
                    onClick={onCancel}
                    className="flex-1 bg-gray-700 text-white px-5 py-2 rounded hover:bg-gray-600 transition-colors font-medium"
                >
                    Отмена
                </button>
            </div>
        </form>
    )
}
