import express from "express"
import Task from "../models/Task.js"
import Project from "../models/Project.js"
import User from "../models/User.js"
import { mapTask, mapMany } from "../utils/mappers.js"
import { authenticateToken, requireAdmin } from "../middleware/auth.js"

const router = express.Router()

router.get("/", authenticateToken, async (req, res) => {
    try {
        const { projectId, status } = req.query
        const filter = {}

        if (projectId) {
            filter.projectId = projectId
        }

        if (status) {
            filter.status = status
        }

        if (req.user.role !== 'admin') {
            filter.user = req.user.userId
        }

        const tasks = await Task.find(filter)
            .populate('projectId', 'name')
            .populate('user', 'login role');

        res.json(mapMany(tasks, mapTask))
    } catch (error) {
        res.status(500).json({ error: error.message })
    }
})

router.post("/", authenticateToken, requireAdmin, async (req, res) => {
    try {
        const { title, description, user, status, projectId, attachments } = req.body

        if (!title) {
            return res.status(400).json({ error: "Название задачи обязательно" })
        }

        if (!user) {
            return res.status(400).json({ error: "Логин пользователя обязателен" })
        }

        if (!projectId) {
            return res.status(400).json({ error: "ID проекта обязателен" })
        }

        const project = await Project.findById(projectId)
        if (!project) {
            return res.status(404).json({ error: "Проект не найден" })
        }

        const userDoc = await User.findOne({ login: user })
        if (!userDoc) {
            return res.status(404).json({ error: `Пользователь с логином "${user}" не найден` })
        }

        const validStatuses = ["todo", "in_progress", "done"]
        const taskStatus = status || "todo"
        if (!validStatuses.includes(taskStatus)) {
            return res.status(400).json({ error: "Недопустимый статус задачи" })
        }

        const newTask = new Task({
            title,
            description: description || "",
            user: userDoc._id,
            status: taskStatus,
            projectId,
            attachments: attachments || []
        })

        const task = await newTask.save()

        await User.findByIdAndUpdate(userDoc._id, {
            $addToSet: { tasks: task._id }
        })

        await task.populate('user', 'login role')
        res.status(201).json(mapTask(task))
    } catch (error) {
        console.error('Ошибка создания задачи:', error)
        res.status(500).json({ error: error.message || 'Ошибка сервера при создании задачи' })
    }
})

router.put("/:id", authenticateToken, async (req, res) => {
    try {
        const { title, description, user, status, projectId, attachments } = req.body

        const task = await Task.findById(req.params.id)
        if (!task) {
            return res.status(404).json({ error: "Task not found" })
        }

        if (req.user.role !== 'admin' && task.user.toString() !== req.user.userId.toString()) {
            return res.status(403).json({ error: "Access denied. You can only update your own tasks." })
        }

        const updates = {}

        if (req.user.role === 'admin') {
            if (title) updates.title = title
            if (description !== undefined) updates.description = description
            if (projectId) {
                const project = await Project.findById(projectId)
                if (!project) {
                    return res.status(404).json({ error: "Project not found" })
                }
                updates.projectId = projectId
            }
            if (user) {
                const userDoc = await User.findById(user)
                if (!userDoc) {
                    return res.status(404).json({ error: "User not found" })
                }

                const oldTask = await Task.findById(req.params.id)
                if (oldTask && oldTask.user.toString() !== user) {
                    await User.findByIdAndUpdate(oldTask.user, {
                        $pull: { tasks: req.params.id }
                    })
                    await User.findByIdAndUpdate(user, {
                        $addToSet: { tasks: req.params.id }
                    })
                }
                updates.user = user
            }
        }

        if (status) {
            const validStatuses = ["todo", "in_progress", "done"]
            if (!validStatuses.includes(status)) {
                return res.status(400).json({ error: "Invalid status" })
            }
            updates.status = status
        }

        if (attachments !== undefined) {
            updates.attachments = attachments
        }

        const updatedTask = await Task.findByIdAndUpdate(
            req.params.id,
            updates,
            { new: true, runValidators: true }
        ).populate('projectId', 'name').populate('user', 'login role');

        res.json(mapTask(updatedTask))
    } catch (error) {
        res.status(500).json({ error: error.message })
    }
})

router.delete("/:id", authenticateToken, requireAdmin, async (req, res) => {
    try {
        const task = await Task.findById(req.params.id)
        if (!task) {
            return res.status(404).json({ error: "Task not found" })
        }

        await User.findByIdAndUpdate(task.user, {
            $pull: { tasks: req.params.id }
        })

        await Task.findByIdAndDelete(req.params.id)

        res.status(204).send()
    } catch (error) {
        res.status(500).json({ error: error.message })
    }
})

export default router
