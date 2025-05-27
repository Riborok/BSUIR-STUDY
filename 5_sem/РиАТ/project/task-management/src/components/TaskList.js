import React, { useState } from 'react';
import EditTaskModal from "./EditTaskModal";
import TaskListItems from "./TaskListItems";
import TaskHistoryModal from "./TaskHistoryModal";
import {useAuth} from "../providers/AuthProvider";

const TaskList = ({ tasks, onDeleteTask, onUpdateTask }) => {
    const { token, clearToken } = useAuth();
    const [tasksVisible, setTasksVisible] = useState(true);
    const [selectedTask, setSelectedTask] = useState(null);
    const [editFormVisible, setEditFormVisible] = useState(false);
    const [editedTask, setEditedTask] = useState({
        title: '',
        description: '',
        status: '',
        participants: [],
    });
    const [taskHistory, setTaskHistory] = useState([]);
    const [historyVisible, setHistoryVisible] = useState(false);

    const fetchTaskHistory = async (taskId) => {
        try {
            const response = await fetch(`http://localhost:3011/api/tasks/${taskId}/history`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token}`,
                },
            });

            if (response.ok) {
                const history = await response.json();
                setTaskHistory(history);
                setHistoryVisible(true);
            } else {
                console.error('Failed to fetch task history.');
            }
        } catch (error) {
            console.error('Error fetching task history:', error);
        }
    };

    const handleTaskClick = (task) => {
        setSelectedTask(task);
        fetchTaskHistory(task.id);
    };

    const handleEditClick = (task) => {
        setEditedTask({
            title: '',
            description: '',
            status: '',
            participants: [],
        });
        setSelectedTask(task);
        setEditFormVisible(true);
    };

    const handleEditSubmit = async (e) => {
        e.preventDefault();
        await onUpdateTask(selectedTask.id, editedTask);
        setEditFormVisible(false);
        setSelectedTask(null);
    };

    return (
        <div className="card shadow-sm p-4">
            <h2>Your Tasks</h2>
            <button
                className="btn btn-outline-secondary mb-3"
                onClick={() => setTasksVisible((prev) => !prev)}
            >
                {tasksVisible ? 'Hide Tasks' : 'Show Tasks'}
            </button>

            {tasksVisible && (
                tasks.length === 0 ? (
                    <p>You don't have any tasks yet.</p>
                ) : (
                    <TaskListItems
                        tasks={tasks}
                        onTaskClick={handleTaskClick}
                        onEditClick={handleEditClick}
                        onDeleteClick={onDeleteTask}
                    />
                )
            )}

            <EditTaskModal
                visible={editFormVisible}
                task={selectedTask}
                onClose={() => setEditFormVisible(false)}
                onSubmit={handleEditSubmit}
                editedTask={editedTask}
                setEditedTask={setEditedTask}
            />

            {historyVisible && (
                <TaskHistoryModal
                    visible={historyVisible}
                    onClose={() => setHistoryVisible(false)}
                    history={taskHistory}
                />
            )}
        </div>
    );
};

export default TaskList;
