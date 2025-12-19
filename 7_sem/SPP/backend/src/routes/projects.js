import express from "express"
import Project from "../models/Project.js"
import User from "../models/User.js"
import { mapProject, mapMany } from "../utils/mappers.js"
import { authenticateToken, requireAdmin } from "../middleware/auth.js"

const router = express.Router()

router.get("/", authenticateToken, async (req, res) => {
    try {
        let projects;

        if (req.user.role === 'admin') {
            projects = await Project.find().populate('users', 'login role');
        } else {
            projects = await Project.find({ users: req.user.userId }).populate('users', 'login role');
        }

        res.json(mapMany(projects, mapProject))
    } catch (error) {
        res.status(500).json({ error: error.message })
    }
})

router.post("/", authenticateToken, requireAdmin, async (req, res) => {
    try {
        const { name, description, users } = req.body

        if (!name) {
            return res.status(400).json({ error: "Название проекта обязательно" })
        }

        let userIds = []

        if (users && users.length > 0) {
            const dbUsers = await User.find({ login: { $in: users } })

            if (dbUsers.length !== users.length) {
                const foundLogins = dbUsers.map(u => u.login)
                const notFoundLogins = users.filter(login => !foundLogins.includes(login))
                return res.status(404).json({
                    error: `Пользователи не найдены: ${notFoundLogins.join(', ')}`
                })
            }

            userIds = dbUsers.map(u => u._id)
        }

        const newProject = new Project({
            name,
            description: description || '',
            users: userIds
        })

        const project = await newProject.save()

        if (userIds.length > 0) {
            await User.updateMany(
                { _id: { $in: userIds } },
                { $addToSet: { projects: project._id } }
            )
        }

        await project.populate('users', 'login role')
        res.status(201).json(mapProject(project))
    } catch (error) {
        console.error('Ошибка создания проекта:', error)
        res.status(500).json({ error: error.message || 'Ошибка сервера при создании проекта' })
    }
})

router.put("/:id", authenticateToken, requireAdmin, async (req, res) => {
    try {
        const { name, description, users } = req.body

        const updates = {}
        if (name) updates.name = name
        if (description !== undefined) updates.description = description
        if (users !== undefined) {
            const oldProject = await Project.findById(req.params.id)
            if (oldProject) {
                await User.updateMany(
                    { _id: { $in: oldProject.users } },
                    { $pull: { projects: req.params.id } }
                )
                await User.updateMany(
                    { _id: { $in: users } },
                    { $addToSet: { projects: req.params.id } }
                )
            }
            updates.users = users
        }

        const updatedProject = await Project.findByIdAndUpdate(
            req.params.id,
            updates,
            { new: true, runValidators: true }
        ).populate('users', 'login role');

        if (!updatedProject) {
            return res.status(404).json({ error: "Project not found" })
        }

        res.json(mapProject(updatedProject))
    } catch (error) {
        res.status(500).json({ error: error.message })
    }
})

router.delete("/:id", authenticateToken, requireAdmin, async (req, res) => {
    try {
        const project = await Project.findByIdAndDelete(req.params.id)
        if (!project) {
            return res.status(404).json({ error: "Project not found" })
        }

        const Task = (await import("../models/Task.js")).default
        const tasks = await Task.find({ projectId: req.params.id })
        const taskIds = tasks.map(t => t._id)

        await Task.deleteMany({ projectId: req.params.id })

        await User.updateMany(
            { projects: req.params.id },
            { $pull: { projects: req.params.id } }
        )
        await User.updateMany(
            { tasks: { $in: taskIds } },
            { $pull: { tasks: { $in: taskIds } } }
        )

        res.status(204).send()
    } catch (error) {
        res.status(500).json({ error: error.message })
    }
})

export default router
