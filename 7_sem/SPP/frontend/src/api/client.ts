import { Project, TaskItem, Attachment } from "../utils/tasks"

const API_BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:5000/api"

export const tokenStorage = {
    getAccessToken: () => localStorage.getItem('accessToken'),
    getRefreshToken: () => localStorage.getItem('refreshToken'),
    setTokens: (accessToken: string, refreshToken: string) => {
        localStorage.setItem('accessToken', accessToken);
        localStorage.setItem('refreshToken', refreshToken);
    },
    clearTokens: () => {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('user');
    },
    getUser: () => {
        const user = localStorage.getItem('user');
        return user ? JSON.parse(user) : null;
    },
    setUser: (user: any) => {
        localStorage.setItem('user', JSON.stringify(user));
    }
};

const getAuthHeaders = () => {
    const token = tokenStorage.getAccessToken();
    return {
        'Content-Type': 'application/json',
        ...(token ? { 'Authorization': `Bearer ${token}` } : {})
    };
};

const refreshAccessToken = async (): Promise<string | null> => {
    const refreshToken = tokenStorage.getRefreshToken();
    if (!refreshToken) return null;

    try {
        const response = await fetch(`${API_BASE_URL}/auth/refresh`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ refreshToken })
        });

        if (!response.ok) {
            tokenStorage.clearTokens();
            return null;
        }

        const data = await response.json();
        tokenStorage.setTokens(data.accessToken, refreshToken);
        if (data.user) tokenStorage.setUser(data.user);
        return data.accessToken;
    } catch (error) {
        tokenStorage.clearTokens();
        return null;
    }
};

const fetchWithAuth = async (url: string, options: RequestInit = {}): Promise<Response> => {
    const headers = getAuthHeaders();
    let response = await fetch(url, {
        ...options,
        headers: { ...headers, ...options.headers }
    });

    if (response.status === 401) {
        const newToken = await refreshAccessToken();
        if (newToken) {
            response = await fetch(url, {
                ...options,
                headers: { ...getAuthHeaders(), ...options.headers }
            });
        } else {
            window.location.href = '/';
        }
    }

    return response;
};

export const authApi = {
    register: async (login: string, password: string, role?: string) => {
        const response = await fetch(`${API_BASE_URL}/auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ login, password, role })
        });
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Ошибка регистрации');
        }
        return response.json();
    },

    login: async (login: string, password: string) => {
        const response = await fetch(`${API_BASE_URL}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ login, password })
        });
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Ошибка входа');
        }
        const data = await response.json();
        tokenStorage.setTokens(data.accessToken, data.refreshToken);
        tokenStorage.setUser(data.user);
        return data;
    },

    logout: () => {
        tokenStorage.clearTokens();
    },

    getCurrentUser: async () => {
        const response = await fetchWithAuth(`${API_BASE_URL}/auth/me`);
        if (!response.ok) throw new Error('Не удалось получить данные пользователя');
        return response.json();
    },

    getUsers: async () => {
        const response = await fetchWithAuth(`${API_BASE_URL}/auth/users`);
        if (!response.ok) throw new Error('Не удалось получить список пользователей');
        const data = await response.json();
        return data.users;
    }
};

export const projectsApi = {
    getAll: async (): Promise<Project[]> => {
        const response = await fetchWithAuth(`${API_BASE_URL}/projects`)
        if (!response.ok) throw new Error("Failed to fetch projects")
        return response.json()
    },

    create: async (name: string, description?: string, userLogins?: string[]): Promise<Project> => {
        const response = await fetchWithAuth(`${API_BASE_URL}/projects`, {
            method: "POST",
            body: JSON.stringify({ name, description, userLogins }),
        })
        if (!response.ok) {
            const errorData = await response.json().catch(() => ({}));
            console.error('Failed to create project:', response.status, errorData);
            throw new Error(errorData.error || errorData.message || "Failed to create project")
        }
        return response.json()
    },

    update: async (id: string, name: string, description?: string): Promise<Project> => {
        const response = await fetchWithAuth(`${API_BASE_URL}/projects/${id}`, {
            method: "PUT",
            body: JSON.stringify({ name, description }),
        })
        if (!response.ok) throw new Error("Failed to update project")
        return response.json()
    },

    delete: async (id: string): Promise<void> => {
        const response = await fetchWithAuth(`${API_BASE_URL}/projects/${id}`, {
            method: "DELETE",
        })
        if (!response.ok) throw new Error("Failed to delete project")
    },
}

export const tasksApi = {
    getAll: async (projectId?: string, status?: string): Promise<TaskItem[]> => {
        const params = new URLSearchParams()
        if (projectId) params.append("projectId", projectId)
        if (status) params.append("status", status)

        const url = `${API_BASE_URL}/tasks${params.toString() ? `?${params.toString()}` : ""}`
        const response = await fetchWithAuth(url)
        if (!response.ok) throw new Error("Failed to fetch tasks")
        return response.json()
    },

    create: async (task: { title: string; description: string; user: string; status: string; projectId: string; attachments?: Attachment[] }): Promise<TaskItem> => {
        const response = await fetchWithAuth(`${API_BASE_URL}/tasks`, {
            method: "POST",
            body: JSON.stringify(task),
        })
        if (!response.ok) {
            const errorData = await response.json().catch(() => ({}));
            console.error('Failed to create task:', response.status, errorData);
            throw new Error(errorData.error || errorData.message || "Failed to create task")
        }
        return response.json()
    },

    update: async (id: string, updates: { title?: string; description?: string; user?: string; status?: string; projectId?: string; attachments?: Attachment[] }): Promise<TaskItem> => {
        const response = await fetchWithAuth(`${API_BASE_URL}/tasks/${id}`, {
            method: "PUT",
            body: JSON.stringify(updates),
        })
        if (!response.ok) {
            const errorData = await response.json().catch(() => ({}));
            console.error('Failed to update task:', response.status, errorData);
            throw new Error(errorData.error || errorData.message || "Failed to update task")
        }
        return response.json()
    },

    delete: async (id: string): Promise<void> => {
        const response = await fetchWithAuth(`${API_BASE_URL}/tasks/${id}`, {
            method: "DELETE",
        })
        if (!response.ok) {
            const errorData = await response.json().catch(() => ({}));
            console.error('Failed to delete task:', response.status, errorData);
            throw new Error(errorData.error || errorData.message || "Failed to delete task")
        }
    },
}
