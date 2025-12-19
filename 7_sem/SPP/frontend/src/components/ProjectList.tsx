import { type Project, type TaskItem, getTaskCountByProject } from "../utils/tasks"
import ProjectCard from "./ProjectCard"

interface ProjectListProps {
    projects: Project[]
    tasks: TaskItem[]
    onProjectClick: (projectId: string) => void
}

export default function ProjectList({ projects, tasks, onProjectClick }: ProjectListProps) {
    if (projects.length === 0) {
        return (
            <div className="text-center py-16">
                <h3 className="text-lg font-semibold text-gray-400 mb-2">Пока нет проектов</h3>
                <p className="text-gray-500">Создайте свой первый проект</p>
            </div>
        )
    }

    return (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
            {projects.map((project) => (
                <ProjectCard
                    key={project.id}
                    project={project}
                    taskCount={getTaskCountByProject(tasks, project.id)}
                    onClick={() => onProjectClick(project.id)}
                />
            ))}
        </div>
    )
}
