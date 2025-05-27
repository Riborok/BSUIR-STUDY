import React from 'react';
import TaskItem from "./TaskItem";

const TaskListItems = ({ tasks, onTaskClick, onEditClick, onDeleteClick }) => (
    <ul className="list-group">
        {tasks.map((task) => (
            <TaskItem
                key={task.id}
                task={task}
                onTaskClick={onTaskClick}
                onEditClick={onEditClick}
                onDeleteClick={onDeleteClick}
            />
        ))}
    </ul>
);

export default TaskListItems;