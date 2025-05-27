import React, { useEffect, useState } from 'react';
import { useAuth } from '../providers/AuthProvider';
import LoadingPage from "./LoadingPage";
import '../css/HomePage.css';
import ErrorAlert from "../components/ErrorAlert";
import TaskForm from "../components/TaskForm";
import TaskList from "../components/TaskList";

const HomePage = () => {
    const { token, parsedToken, clearToken } = useAuth();
    const [tasks, setTasks] = useState([]);
    const [loading, setLoading] = useState(true);
    const [errorMessage, setErrorMessage] = useState(null);

    const handleErrorClose = () => setErrorMessage(null);

    useEffect(() => {
        const fetchTasks = async () => {
            try {
                const response = await fetch('http://localhost:3011/api/tasks', {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                    },
                });

                if (response.ok) {
                    const data = await response.json();
                    setTasks(data);
                } else if (response.status === 403) {
                    clearToken();
                } else {
                    const errorData = await response.text();
                    setErrorMessage(errorData || 'Error getting tasks!');
                }

            } catch (err) {
                setErrorMessage('Server Connection Error!');
            } finally {
                setLoading(false);
            }
        };

        fetchTasks();
    }, [token, clearToken]);

    const handleAddTask = async (newTask) => {
        try {
            const response = await fetch('http://localhost:3011/api/tasks', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(newTask),
            });

            if (response.ok) {
                const createdTask = await response.json();
                setTasks((prevTasks) => [...prevTasks, createdTask]);
            } else if (response.status === 403) {
                clearToken();
            } else {
                const errorData = await response.text();
                setErrorMessage(errorData || 'Failed to add task!');
            }
        } catch (err) {
            setErrorMessage('Server Connection Error!');
        }
    };

    const handleDeleteTask = async (taskId) => {
        try {
            const response = await fetch(`http://localhost:3011/api/tasks/${taskId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`,
                },
            });

            if (response.ok) {
                setTasks((prevTasks) => prevTasks.filter((task) => task.id !== taskId));
            } else if (response.status === 403) {
                clearToken();
            } else {
                const errorData = await response.text();
                setErrorMessage(errorData || 'Failed to delete task!');
            }
        } catch (err) {
            setErrorMessage('Server Connection Error!');
        }
    };

    const handleUpdateTask = async (taskId, updatedTask) => {
        try {
            if (updatedTask.participants.length !== 0) {
                const response = await fetch(`http://localhost:3011/api/tasks/${taskId}/participants`, {
                    method: 'PUT',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(updatedTask.participants),
                });

                if (response.status !== 204) {
                    if (response.status === 403) {
                        clearToken();
                        return;
                    }
                    const errorData = await response.text();
                    setErrorMessage(errorData || 'Failed to update participants!');
                    return;
                }
            }

            if (updatedTask.status) {
                const response = await fetch(`http://localhost:3011/api/tasks/${taskId}/status?newStatus=${updatedTask.status}`, {
                    method: 'PUT',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                    },
                });

                if (response.status !== 204) {
                    if (response.status === 403) {
                        clearToken();
                        return;
                    }
                    const errorData = await response.text();
                    setErrorMessage(errorData || 'Failed to update task status!');
                    return;
                }
            }

            if (updatedTask.description) {
                const response = await fetch(`http://localhost:3011/api/tasks/${taskId}/description?newDescription=${updatedTask.description}`, {
                    method: 'PUT',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                    },
                });

                if (response.status !== 204) {
                    if (response.status === 403) {
                        clearToken();
                        return;
                    }
                    const errorData = await response.text();
                    setErrorMessage(errorData || 'Failed to update task description!');
                    return;
                }
            }

            if (updatedTask.title) {
                const response = await fetch(`http://localhost:3011/api/tasks/${taskId}/title?newTitle=${updatedTask.title}`, {
                    method: 'PUT',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                    },
                });

                if (response.status !== 204) {
                    if (response.status === 403) {
                        clearToken();
                        return;
                    }
                    const errorData = await response.text();
                    setErrorMessage(errorData || 'Failed to update task title!');
                    return;
                }
            }

            const cleanedUpdatedTask = Object.fromEntries(
                Object.entries(updatedTask).filter(([key, value]) => {
                    return Boolean(value) && !(Array.isArray(value) && value.length === 0);
                })
            );
            setTasks((prevTasks) =>
                prevTasks.map((task) => (task.id === taskId ? { ...task, ...cleanedUpdatedTask } : task))
            );
        } catch (err) {
            setErrorMessage('Server Connection Error!');
        }
    };

    return (
        loading
            ? <LoadingPage />
            : <div className="container mt-5">
                <ErrorAlert message={errorMessage} onClose={handleErrorClose} />

                <div className="card shadow-sm p-4 mb-4">
                    <h2>User Profile</h2>
                    <div className="mb-3">
                        <p><strong>Name:</strong> {parsedToken?.firstName} {parsedToken?.lastName}</p>
                        <p><strong>Email:</strong> {parsedToken?.sub}</p>
                    </div>
                    <button className="btn btn-danger" onClick={clearToken}>Logout</button>
                </div>

                <TaskForm onAddTask={handleAddTask} />
                <TaskList tasks={tasks} onDeleteTask={handleDeleteTask} onUpdateTask={handleUpdateTask} />
            </div>
    );
};

export default HomePage;
