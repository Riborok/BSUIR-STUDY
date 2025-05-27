import React from 'react';

const TaskHistoryModal = ({ visible, history, onClose }) => {
    if (!visible) return null;

    return (
        <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
            <div className="modal-dialog">
                <div className="modal-content">
                    <div className="modal-header">
                        <h5 className="modal-title">Task History</h5>
                        <button type="button" className="btn-close" onClick={onClose}></button>
                    </div>
                    <div className="modal-body">
                        {history.length === 0 ? (
                            <p>No history available for this task.</p>
                        ) : (
                            <ul className="list-group">
                                {history.map((entry, index) => (
                                    <li key={index} className="list-group-item">
                                        <p>
                                            <strong>Action:</strong> {entry.action}
                                        </p>
                                        <p>
                                            <strong>Previous Value:</strong> {entry.previousValue}
                                        </p>
                                        <p>
                                            <strong>New Value:</strong> {entry.newValue}
                                        </p>
                                        <p>
                                            <strong>Changed By:</strong> {entry.changedBy}
                                        </p>
                                        <p>
                                            <strong>Changed At:</strong> {new Date(entry.changedAt).toLocaleString()}
                                        </p>
                                    </li>
                                ))}
                            </ul>
                        )}
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-secondary" onClick={onClose}>
                            Close
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default TaskHistoryModal;