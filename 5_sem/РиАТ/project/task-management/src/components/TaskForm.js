import React, { useState } from 'react';

const TaskForm = ({ onAddTask }) => {
    const [formVisible, setFormVisible] = useState(false);
    const [newTask, setNewTask] = useState({
        title: '',
        description: '',
        status: 'PENDING',
        participants: [],
    });

    const handleSubmit = (e) => {
        e.preventDefault();
        onAddTask(newTask);
        setNewTask({ title: '', description: '', status: 'PENDING', participants: [] });
        setFormVisible(false);
    };

    return (
        <div className="card shadow-sm p-4 mb-4">
            <h2>Add Task</h2>
            <button
                className="btn btn-outline-primary mb-3"
                onClick={() => setFormVisible((prev) => !prev)}
            >
                {formVisible ? 'Hide Form' : 'Add Task'}
            </button>

            {formVisible && (
                <form onSubmit={handleSubmit}>
                    <div className="mb-3">
                        <label htmlFor="taskTitle" className="form-label">Title</label>
                        <input
                            type="text"
                            id="taskTitle"
                            className="form-control"
                            value={newTask.title}
                            onChange={(e) => setNewTask({ ...newTask, title: e.target.value })}
                            required
                        />
                    </div>
                    <div className="mb-3">
                        <label htmlFor="taskDescription" className="form-label">Description</label>
                        <textarea
                            id="taskDescription"
                            className="form-control"
                            rows="3"
                            value={newTask.description}
                            onChange={(e) => setNewTask({ ...newTask, description: e.target.value })}
                            required
                        ></textarea>
                    </div>
                    <div className="mb-3">
                        <label htmlFor="taskStatus" className="form-label">Status</label>
                        <select
                            id="taskStatus"
                            className="form-control"
                            value={newTask.status}
                            onChange={(e) => setNewTask({ ...newTask, status: e.target.value })}
                            required
                        >
                            <option value="PENDING">Pending</option>
                            <option value="IN_PROGRESS">In Progress</option>
                            <option value="COMPLETED">Completed</option>
                        </select>
                    </div>
                    <div className="mb-3">
                        <label htmlFor="taskParticipants" className="form-label">Participants (comma-separated)</label>
                        <input
                            type="text"
                            id="taskParticipants"
                            className="form-control"
                            value={newTask.participants.join(', ')}
                            onChange={(e) => setNewTask({ ...newTask, participants: e.target.value.split(',').map(p => p.trim()) })}
                        />
                    </div>
                    <button type="submit" className="btn btn-primary">Add</button>
                </form>
            )}
        </div>
    );
};

export default TaskForm;
