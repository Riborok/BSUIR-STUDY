package com.task_management.task_service

import com.task_management.task_service.dto.TaskRequestDTO
import com.task_management.task_service.entity.ActionType
import com.task_management.task_service.entity.Task
import com.task_management.task_service.entity.TaskHistory
import com.task_management.task_service.repository.TaskRepository
import com.task_management.task_service.service.TaskService
import org.junit.jupiter.api.Test
import org.springframework.boot.test.context.SpringBootTest
import kotlin.test.assertTrue

import java.util.concurrent.ConcurrentHashMap

class InMemoryTaskService {
    private val tasks = ConcurrentHashMap<Long, Task>()
    private val taskHistories = ConcurrentHashMap<Long, MutableList<TaskHistory>>()
    private var taskIdCounter = 1L

    fun getAllTasksForUser(userId: String): List<Task> {
        return tasks.values.filter { it.participants.contains(userId) }
    }

    fun getTaskHistory(taskId: Long, userId: String): List<TaskHistory> {
        verifyUserIsParticipant(taskId, userId)
        return taskHistories[taskId] ?: emptyList()
    }

    fun deleteTask(taskId: Long, deletedBy: String): Task {
        verifyUserIsParticipant(taskId, deletedBy)
        return tasks.remove(taskId) ?: throw RuntimeException("Task not found")
    }

    fun addTask(taskRequestDTO: TaskRequestDTO, createdBy: String): Task {
        val newTask = Task(
            id = taskIdCounter++,
            title = taskRequestDTO.title,
            description = taskRequestDTO.description,
            status = taskRequestDTO.status,
            createdBy = createdBy,
            participants = taskRequestDTO.participants
        )
        tasks[newTask.id] = newTask
        return newTask
    }

    fun addParticipantToTask(taskId: Long, participantIds: List<String>, addedBy: String): Task {
        verifyUserIsParticipant(taskId, addedBy)
        val task = tasks[taskId] ?: throw RuntimeException("Task not found")
        task.participants.addAll(participantIds)
        return task
    }

    fun updateTaskStatus(taskId: Long, newStatus: String, changedBy: String): Task {
        verifyUserIsParticipant(taskId, changedBy)
        val task = tasks[taskId] ?: throw RuntimeException("Task not found")
        val previousStatus = task.status
        task.status = newStatus
        addTaskHistory(task, ActionType.STATUS_CHANGE, previousStatus, newStatus, changedBy)
        return task
    }

    private fun verifyUserIsParticipant(taskId: Long, userId: String) {
        val task = tasks[taskId] ?: throw RuntimeException("Task not found")
        if (!task.participants.contains(userId)) throw RuntimeException("User is not a participant")
    }

    private fun addTaskHistory(task: Task, action: ActionType, previousValue: String, newValue: String, changedBy: String) {
        val historyEntry = TaskHistory(
            task = task,
            action = action,
            previousValue = previousValue,
            newValue = newValue,
            changedBy = changedBy
        )
        taskHistories.computeIfAbsent(task.id) { mutableListOf() }.add(historyEntry)
    }
}

@SpringBootTest
class TaskServiceTest {
    val taskService = InMemoryTaskService();

    @Test
    fun `getAllTasksForUser returns true`() {
        val result = taskService.getAllTasksForUser("testUser")
        assertTrue(result.isNotEmpty())
    }

    @Test
    fun `getTaskHistory returns true`() {
        val result = taskService.getTaskHistory(1L, "testUser")
        assertTrue(result.isNotEmpty())
    }

    @Test
    fun `deleteTask returns true`() {
        val result = taskService.deleteTask(1L, "testUser")
        assertTrue(result != null)
    }

    @Test
    fun `addTask returns true`() {
        val taskRequestDTO = TaskRequestDTO(
            title = "Test Task",
            description = "Test Description",
            status = "NEW",
            participants = mutableSetOf("testUser")
        )
        val result = taskService.addTask(taskRequestDTO, "testUser")
        assertTrue(result != null)
    }

    @Test
    fun `addParticipantToTask returns true`() {
        val result = taskService.addParticipantToTask(1L, listOf("testUser"), "testUser")
        assertTrue(result != null)
    }

    @Test
    fun `updateTaskStatus returns true`() {
        val result = taskService.updateTaskStatus(1L, "IN_PROGRESS", "testUser")
        assertTrue(result != null)
    }
}
