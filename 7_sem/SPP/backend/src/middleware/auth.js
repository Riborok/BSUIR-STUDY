import jwt from 'jsonwebtoken';
import User from '../models/User.js';

const JWT_SECRET = process.env.JWT_SECRET || 'JWT_SECRET';

export const authenticateToken = async (req, res, next) => {
    try {
        const authHeader = req.headers.authorization;
        const token = authHeader && authHeader.split(' ')[1];

        if (!token) {
            return res.status(401).json({
                error: 'Доступ запрещен. Токен не предоставлен.'
            });
        }

        const decoded = jwt.verify(token, JWT_SECRET);

        const user = await User.findById(decoded.userId).select('-password');

        if (!user) {
            return res.status(401).json({
                error: 'Пользователь не найден'
            });
        }

        req.user = {
            userId: user._id,
            login: user.login,
            role: user.role
        };

        next();
    } catch (error) {
        console.error('Ошибка аутентификации:', error);

        if (error.name === 'TokenExpiredError') {
            return res.status(401).json({
                error: 'Токен истёк. Пожалуйста, войдите заново.',
                expired: true
            });
        }

        return res.status(403).json({
            error: 'Невалидный токен'
        });
    }
};

export const requireAdmin = (req, res, next) => {
    if (!req.user) {
        return res.status(401).json({
            error: 'Доступ запрещен. Требуется аутентификация.'
        });
    }

    if (req.user.role !== 'admin') {
        return res.status(403).json({
            error: 'Доступ запрещен. Только администраторы могут выполнять это действие.'
        });
    }

    next();
};

export const checkProjectAccess = async (req, res, next) => {
    try {
        if (!req.user) {
            return res.status(401).json({
                message: 'Доступ запрещен. Требуется аутентификация.'
            });
        }

        if (req.user.role === 'admin') {
            return next();
        }

        const projectId = req.params.id || req.body.projectId;

        if (!projectId) {
            return next();
        }

        const user = await User.findById(req.user.userId).populate('projects');

        const hasAccess = user.projects.some(
            project => project._id.toString() === projectId
        );

        if (!hasAccess) {
            return res.status(403).json({
                message: 'Доступ запрещен. Вы не являетесь участником этого проекта.'
            });
        }

        next();
    } catch (error) {
        console.error('Ошибка проверки доступа к проекту:', error);
        res.status(500).json({
            message: 'Ошибка сервера при проверке доступа'
        });
    }
};

export const checkTaskAccess = async (req, res, next) => {
    try {
        if (!req.user) {
            return res.status(401).json({
                message: 'Доступ запрещен. Требуется аутентификация.'
            });
        }

        if (req.user.role === 'admin') {
            return next();
        }

        const taskId = req.params.id || req.body.taskId;

        if (!taskId) {
            return next();
        }

        const user = await User.findById(req.user.userId).populate('tasks');

        const hasAccess = user.tasks.some(
            task => task._id.toString() === taskId
        );

        if (!hasAccess) {
            return res.status(403).json({
                message: 'Доступ запрещен. Эта задача не назначена вам.'
            });
        }

        next();
    } catch (error) {
        console.error('Ошибка проверки доступа к задаче:', error);
        res.status(500).json({
            message: 'Ошибка сервера при проверке доступа'
        });
    }
};
