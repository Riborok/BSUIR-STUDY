import express from 'express';
import bcrypt from 'bcrypt';
import jwt from 'jsonwebtoken';
import User from '../models/User.js';

const router = express.Router();

const JWT_SECRET = process.env.JWT_SECRET || 'JWT_SECRET';
const JWT_REFRESH_SECRET = process.env.JWT_REFRESH_SECRET || 'JWT_REFRESH_SECRET';
const ACCESS_TOKEN_EXPIRY = '15m';
const REFRESH_TOKEN_EXPIRY = '7d';

router.post('/register', async (req, res) => {
    try {
        const { login, password, role } = req.body;

        if (!login || !password) {
            return res.status(400).json({
                message: 'Логин и пароль обязательны'
            });
        }

        const hashedPassword = await bcrypt.hash(password, 10);

        const user = new User({
            login,
            password: hashedPassword,
            role: role || 'user'
        });

        await user.save();

        res.status(201).json({
            user: {
                id: user._id,
                login: user.login,
                role: user.role
            }
        });
    } catch (error) {
        console.error('Ошибка регистрации:', error);
        res.status(500).json({ message: 'Ошибка сервера при регистрации' });
    }
});

router.post('/login', async (req, res) => {
    try {
        const { login, password } = req.body;

        if (!login || !password) {
            return res.status(400).json({
                message: 'Логин и пароль обязательны'
            });
        }

        const user = await User.findOne({ login });
        if (!user) {
            return res.status(401).json({
                message: 'Неверный логин или пароль'
            });
        }

        const isPasswordValid = await bcrypt.compare(password, user.password);
        if (!isPasswordValid) {
            return res.status(401).json({
                message: 'Неверный логин или пароль'
            });
        }

        const accessToken = jwt.sign(
            {
                userId: user._id,
                login: user.login,
                role: user.role
            },
            JWT_SECRET,
            { expiresIn: ACCESS_TOKEN_EXPIRY }
        );

        const refreshToken = jwt.sign(
            {
                userId: user._id,
                login: user.login
            },
            JWT_REFRESH_SECRET,
            { expiresIn: REFRESH_TOKEN_EXPIRY }
        );

        res.json({
            message: 'Вход выполнен успешно',
            accessToken,
            refreshToken,
            user: {
                id: user._id,
                login: user.login,
                role: user.role
            }
        });
    } catch (error) {
        console.error('Ошибка входа:', error);
        res.status(500).json({ message: 'Ошибка сервера при входе' });
    }
});

router.post('/refresh', async (req, res) => {
    try {
        const { refreshToken } = req.body;

        if (!refreshToken) {
            return res.status(401).json({
                message: 'Refresh токен не предоставлен'
            });
        }

        const decoded = jwt.verify(refreshToken, JWT_REFRESH_SECRET);

        const user = await User.findById(decoded.userId);
        if (!user) {
            return res.status(401).json({
                message: 'Пользователь не найден'
            });
        }

        const accessToken = jwt.sign(
            {
                userId: user._id,
                login: user.login,
                role: user.role
            },
            JWT_SECRET,
            { expiresIn: ACCESS_TOKEN_EXPIRY }
        );

        res.json({
            accessToken,
            user: {
                id: user._id,
                login: user.login,
                role: user.role
            }
        });
    } catch (error) {
        console.error('Ошибка обновления токена:', error);
        res.status(401).json({ message: 'Невалидный или истёкший refresh токен' });
    }
});

router.get('/me', async (req, res) => {
    try {
        const token = req.headers.authorization?.split(' ')[1];

        if (!token) {
            return res.status(401).json({
                message: 'Токен не предоставлен'
            });
        }

        const decoded = jwt.verify(token, JWT_SECRET);
        const user = await User.findById(decoded.userId).select('-password');

        if (!user) {
            return res.status(404).json({
                message: 'Пользователь не найден'
            });
        }

        res.json({
            user: {
                id: user._id,
                login: user.login,
                role: user.role
            }
        });
    } catch (error) {
        console.error('Ошибка получения пользователя:', error);
        res.status(401).json({ message: 'Невалидный токен' });
    }
});

router.get('/users', async (req, res) => {
    try {
        const token = req.headers.authorization?.split(' ')[1];

        if (!token) {
            return res.status(401).json({
                message: 'Токен не предоставлен'
            });
        }

        jwt.verify(token, JWT_SECRET);

        const users = await User.find().select('-password');

        res.json({
            users: users.map(user => ({
                id: user._id,
                login: user.login,
                role: user.role
            }))
        });
    } catch (error) {
        console.error('Ошибка получения списка пользователей:', error);
        res.status(401).json({ message: 'Невалидный токен' });
    }
});

export default router;
