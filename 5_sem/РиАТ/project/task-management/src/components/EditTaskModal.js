import React from 'react';

const EditTaskModal = ({
                           visible,
                           task,
                           onClose,
                           onSubmit,
                           editedTask,
                           setEditedTask,
                       }) => {
    if (!visible || !task) return null;

    return (
        <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
            <div className="modal-dialog">
                <div className="modal-content">
                    <div className="modal-header">
                        <h5 className="modal-title">Edit Task</h5>
                        <button type="button" className="btn-close" onClick={onClose}></button>
                    </div>
                    <form onSubmit={onSubmit}>
                        <div className="modal-body">
                            <div className="mb-3">
                                <label htmlFor="editTaskTitle" className="form-label">New Title</label>
                                <input
                                    type="text"
                                    id="editTaskTitle"
                                    className="form-control"
                                    value={editedTask.title}
                                    onChange={(e) =>
                                        setEditedTask({ ...editedTask, title: e.target.value })
                                    }
                                />
                            </div>
                            <div className="mb-3">
                                <label htmlFor="editTaskDescription" className="form-label">New Description</label>
                                <textarea
                                    id="editTaskDescription"
                                    className="form-control"
                                    rows="3"
                                    value={editedTask.description}
                                    onChange={(e) =>
                                        setEditedTask({ ...editedTask, description: e.target.value })
                                    }
                                ></textarea>
                            </div>
                            <div className="mb-3">
                                <label htmlFor="editTaskStatus" className="form-label">New Status</label>
                                <select
                                    id="editTaskStatus"
                                    className="form-control"
                                    value={editedTask.status}
                                    onChange={(e) =>
                                        setEditedTask({ ...editedTask, status: e.target.value })
                                    }
                                >
                                    <option value=""></option>
                                    <option value="PENDING">Pending</option>
                                    <option value="IN_PROGRESS">In Progress</option>
                                    <option value="COMPLETED">Completed</option>
                                </select>
                            </div>
                            <div className="mb-3">
                                <label htmlFor="editTaskParticipants" className="form-label">New Participants (comma-separated)</label>
                                <input
                                    type="text"
                                    id="editTaskParticipants"
                                    className="form-control"
                                    value={editedTask.participants.join(', ')}
                                    onChange={(e) =>
                                        setEditedTask({
                                            ...editedTask,
                                            participants: e.target.value.split(',').map((p) => p.trim()),
                                        })
                                    }
                                />
                            </div>
                        </div>
                        <div className="modal-footer">
                            <button type="button" className="btn btn-secondary" onClick={onClose}>
                                Close
                            </button>
                            <button type="submit" className="btn btn-primary">Save changes</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
};

export default EditTaskModal;