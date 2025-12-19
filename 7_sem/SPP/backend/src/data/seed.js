import bcrypt from 'bcrypt';
import mongoose from 'mongoose';
import User from '../models/User.js';
import Project from '../models/Project.js';
import Task from '../models/Task.js';

const MONGODB_URI = process.env.MONGODB_URI || 'mongodb://localhost:27017/spp_db';

const seedDatabase = async () => {
    try {
        await mongoose.connect(MONGODB_URI);

        await User.deleteMany({});
        await Project.deleteMany({});
        await Task.deleteMany({});
        const users = await User.create([
            {
                login: 'admin',
                password: await bcrypt.hash('123', 10),
                role: 'admin'
            },
            {
                login: 'ivan',
                password: await bcrypt.hash('123', 10),
                role: 'user'
            },
            {
                login: 'maria',
                password: await bcrypt.hash('123', 10),
                role: 'user'
            },
            {
                login: 'petr',
                password: await bcrypt.hash('123', 10),
                role: 'user'
            },
            {
                login: 'anna',
                password: await bcrypt.hash('123', 10),
                role: 'user'
            }
        ]);

        const projects = await Project.create([
            {
                name: 'Корпоративный веб-портал',
                description: 'Разработка внутреннего веб-портала для сотрудников компании с модулями HR, документооборота и коммуникации',
                users: [users[1]._id, users[2]._id, users[3]._id]
            },
            {
                name: 'Мобильное приложение доставки',
                description: 'iOS и Android приложение для заказа и отслеживания доставки еды',
                users: [users[2]._id, users[4]._id]
            },
            {
                name: 'Система аналитики',
                description: 'Dashboard для визуализации бизнес-метрик и отчетности в реальном времени',
                users: [users[1]._id, users[3]._id, users[4]._id]
            }
        ]);

        const tasks = await Task.create([
            {
                title: 'Разработка дизайн-макетов',
                description: 'Создать макеты всех основных страниц портала в Figma',
                user: users[1]._id,
                status: 'done',
                projectId: projects[0]._id,
                attachments: [
                ]
            },
            {
                title: 'Настройка backend API',
                description: 'Реализовать REST API для модулей HR и документооборота',
                user: users[2]._id,
                status: 'in_progress',
                projectId: projects[0]._id,
                attachments: [
                ]
            },
            {
                title: 'Интеграция с Active Directory',
                description: 'Настроить SSO авторизацию через корпоративный AD',
                user: users[3]._id,
                status: 'todo',
                projectId: projects[0]._id
            },
            {
                title: 'Разработка модуля чата',
                description: 'Реализовать внутренний мессенджер с поддержкой групповых чатов',
                user: users[2]._id,
                status: 'todo',
                projectId: projects[0]._id
            },

            {
                title: 'Дизайн UI/UX мобильного приложения',
                description: 'Создать адаптивный дизайн для iOS и Android',
                user: users[4]._id,
                status: 'done',
                projectId: projects[1]._id,
                attachments: [
                ]
            },
            {
                title: 'Реализация карты и геолокации',
                description: 'Интегрировать Google Maps для отслеживания курьера',
                user: users[2]._id,
                status: 'in_progress',
                projectId: projects[1]._id
            },
            {
                title: 'Система push-уведомлений',
                description: 'Настроить Firebase Cloud Messaging для уведомлений о заказах',
                user: users[4]._id,
                status: 'in_progress',
                projectId: projects[1]._id
            },
            {
                title: 'Интеграция платежных систем',
                description: 'Подключить оплату через карты и электронные кошельки',
                user: users[2]._id,
                status: 'todo',
                projectId: projects[1]._id
            },

            {
                title: 'Настройка ETL процессов',
                description: 'Создать pipelines для загрузки данных из различных источников',
                user: users[3]._id,
                status: 'done',
                projectId: projects[2]._id,
                attachments: [
                ]
            },
            {
                title: 'Разработка дашбордов',
                description: 'Создать интерактивные графики и таблицы для визуализации данных',
                user: users[1]._id,
                status: 'in_progress',
                projectId: projects[2]._id
            },
            {
                title: 'Оптимизация запросов к БД',
                description: 'Улучшить производительность аналитических запросов',
                user: users[3]._id,
                status: 'in_progress',
                projectId: projects[2]._id
            },
            {
                title: 'Экспорт отчетов в Excel/PDF',
                description: 'Добавить функционал выгрузки отчетов в различных форматах',
                user: users[4]._id,
                status: 'todo',
                projectId: projects[2]._id
            }
        ]);

        const tasksByProject = {};
        tasks.forEach(task => {
            const projectId = task.projectId.toString();
            if (!tasksByProject[projectId]) {
                tasksByProject[projectId] = [];
            }
            tasksByProject[projectId].push(task);
        });

        for (const user of users) {
            const userProjects = projects.filter(p =>
                p.users.some(u => u.equals(user._id))
            );
            const userTasks = tasks.filter(t => t.user.equals(user._id));

            await User.findByIdAndUpdate(user._id, {
                projects: userProjects.map(p => p._id),
                tasks: userTasks.map(t => t._id)
            });

        }

        console.log('✅ База данных успешно заполнена!');
        console.log(`Создано пользователей: ${users.length}`);
        console.log(`Создано проектов: ${projects.length}`);
        console.log(`Создано задач: ${tasks.length}`);

        await mongoose.connection.close();
        console.log('Соединение с MongoDB закрыто');
        process.exit(0);
    } catch (error) {
        console.error('Ошибка при заполнении базы данных:', error);
        await mongoose.connection.close();
        process.exit(1);
    }
};

seedDatabase();
