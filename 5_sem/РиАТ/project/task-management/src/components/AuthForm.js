import React from 'react';

const AuthForm = ({ title, buttonText, onSubmit, fields, onInputChange }) => {
    return (
        <div className="card-body">
            <h3 className="card-title text-center mb-4">{title}</h3>
            <form onSubmit={onSubmit}>
                {fields.map((field) => (
                    <div className="mb-3" key={field.id}>
                        <label htmlFor={field.id} className="form-label">
                            {field.label}:
                        </label>
                        <input
                            type={field.type || 'text'}
                            className="form-control"
                            id={field.id}
                            value={field.value}
                            onChange={onInputChange}
                            required={field.required || true}
                        />
                    </div>
                ))}
                <button type="submit" className="btn btn-primary w-100">
                    {buttonText}
                </button>
            </form>
        </div>
    );
};

export default AuthForm;
