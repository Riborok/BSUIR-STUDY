import React from 'react';

const TaskItem = ({ task, onTaskClick, onEditClick, onDeleteClick }) => (
    <li
        className="list-group-item d-flex justify-content-between align-items-center"
        onClick={() => onTaskClick(task)}
        style={{ cursor: 'pointer' }}
    >
        <div className="d-flex flex-column flex-grow-1">
            <h5 className="mb-1">{task.title}</h5>
            <p className="mb-1 text-muted">{task.description}</p>
            <small>Participants: {task.participants.join(', ')}</small>
        </div>

        <span
            className={`badge bg-${
                task.status === 'COMPLETED' ? 'success' : 'warning'
            } ms-3`}
            style={{ whiteSpace: 'nowrap' }}
        >
            {task.status}
        </span>

        <div className="ms-3 d-flex gap-2">
            <button
                className="btn btn-info btn-sm"
                onClick={(e) => {
                    e.stopPropagation();
                    onEditClick(task);
                }}
            >
                Edit
            </button>
            <button
                className="btn btn-danger btn-sm"
                onClick={(e) => {
                    e.stopPropagation();
                    onDeleteClick(task.id);
                }}
            >
                Delete
            </button>
        </div>
    </li>
);

export default TaskItem;