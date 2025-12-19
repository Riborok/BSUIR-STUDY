import { TaskItem, TaskStatus, filterTasksByStatus } from "../utils/tasks"
import TaskCard from "./TaskCard"

interface KanbanBoardProps {
    tasks: TaskItem[]
    onUpdateTask?: (taskId: string, updates: { title?: string; description?: string; user?: string; status?: string; projectId?: string }) => void
    onDeleteTask?: (taskId: string) => void
}

export default function KanbanBoard({ tasks, onUpdateTask, onDeleteTask }: KanbanBoardProps) {
    const columns = [
        { status: "todo" as TaskStatus, title: "To Do", bgColor: "bg-gray-800" },
        { status: "in_progress" as TaskStatus, title: "In Progress", bgColor: "bg-gray-800" },
        { status: "done" as TaskStatus, title: "Done", bgColor: "bg-gray-800" }
    ]

    const renderColumn = (status: TaskStatus, title: string, bgColor: string) => {
        const columnTasks = filterTasksByStatus(tasks, status)

        return (
            <div className="flex-1 min-w-[280px]">
                <div className={`${bgColor} border border-gray-700 rounded-lg p-4`}>
                    <div className="flex items-center justify-between mb-4 pb-3 border-b border-gray-700">
                        <h3 className="font-semibold text-white">{title}</h3>
                        <span className="bg-gray-700 text-white text-xs font-semibold px-2 py-1 rounded">
                            {columnTasks.length}
                        </span>
                    </div>
                    <div className="space-y-3 max-h-[calc(100vh-300px)] overflow-y-auto">
                        {columnTasks.length === 0 ? (
                            <div className="text-center py-8 text-gray-500 text-sm">
                                <div className="text-3xl mb-2">📭</div>
                                <p>Нет задач</p>
                            </div>
                        ) : (
                            columnTasks.map((task) => (
                                <TaskCard
                                    key={task.id}
                                    task={task}
                                    onUpdate={onUpdateTask}
                                    onDelete={onDeleteTask}
                                />
                            ))
                        )}
                    </div>
                </div>
            </div>
        )
    }

    return (
        <div className="flex gap-4 overflow-x-auto pb-4">
            {columns.map((col) => renderColumn(col.status, col.title, col.bgColor))}
        </div>
    )
}
