import { Project } from "../utils/tasks"

interface ProjectCardProps {
    project: Project
    taskCount: number
    onClick: () => void
}

export default function ProjectCard({ project, taskCount, onClick }: ProjectCardProps) {
    return (
        <div
            onClick={onClick}
            className="bg-gray-800 border border-gray-700 rounded-lg p-5 hover:border-violet-600 cursor-pointer transition-colors"
        >
            <div className="flex items-start justify-between mb-3">
                <h3 className="text-lg font-semibold text-white flex-1 pr-2">
                    {project.name}
                </h3>
            </div>

            {project.description && (
                <p className="text-sm text-gray-400 mb-4">
                    {project.description}
                </p>
            )}

            {project.users && project.users.length > 0 && (
                <div className="mb-4">
                    <p className="text-xs text-gray-500 mb-2">Участники:</p>
                    <div className="flex flex-wrap gap-2">
                        {project.users.slice(0, 4).map((user) => (
                            <span
                                key={user.id}
                                className="text-xs bg-gray-700 text-gray-300 px-2 py-1 rounded"
                            >
                                {user.login}
                            </span>
                        ))}
                        {project.users.length > 4 && (
                            <span className="text-xs bg-gray-700 text-gray-400 px-2 py-1 rounded">
                                +{project.users.length - 4}
                            </span>
                        )}
                    </div>
                </div>
            )}

            <div className="flex items-center gap-2 pt-3 border-t border-gray-700">
                <span className="text-gray-400 text-sm">Задач:</span>
                <span className="font-semibold text-violet-400">{taskCount}</span>
            </div>
        </div>
    )
}
