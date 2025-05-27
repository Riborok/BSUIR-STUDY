package com.task_management.task_service.service

import com.task_management.task_service.dto.TaskRequestDTO
import com.task_management.task_service.entity.ActionType
import com.task_management.task_service.entity.Task
import com.task_management.task_service.entity.TaskHistory
import com.task_management.task_service.kafka.EmailValidatorService
import com.task_management.task_service.repository.TaskHistoryRepository
import com.task_management.task_service.repository.TaskRepository
import org.springframework.security.access.AccessDeniedException
import org.springframework.stereotype.Service
import org.springframework.transaction.annotation.Transactional
import java.util.concurrent.TimeUnit
import org.slf4j.LoggerFactory

@Service
@Transactional
class TaskService(
    private val taskRepository: TaskRepository,
    private val taskHistoryRepository: TaskHistoryRepository,
    private val emailValidatorService: EmailValidatorService
) {
    private val logger = LoggerFactory.getLogger(TaskService::class.java)

    fun getAllTasksForUser(userId: String): List<Task> {
        logger.info("Fetching all tasks for user: $userId")
        return taskRepository.findAllTasksForUser(userId)
    }

    fun getTaskHistory(taskId: Long, userId: String): List<TaskHistory> {
        logger.info("Fetching task history for task ID: $taskId, user: $userId")
        val task = taskRepository.findById(taskId).orElseThrow { RuntimeException("Task not found") }
        verifyUserIsParticipant(taskId, userId)
        return task.history
    }

    fun deleteTask(taskId: Long, deletedBy: String): Task {
        logger.info("Attempting to delete task ID: $taskId by user: $deletedBy")
        val task = taskRepository.findById(taskId).orElseThrow { RuntimeException("Task not found") }
        verifyUserIsParticipant(taskId, deletedBy)
        taskRepository.delete(task)
        logger.info("Task ID: $taskId deleted successfully by user: $deletedBy")
        return task
    }

    fun addTask(taskRequestDTO: TaskRequestDTO, createdBy: String): Task {
        logger.info("Adding new task by user: $createdBy with title: ${taskRequestDTO.title}")
        taskRequestDTO.participants.add(createdBy)
        validateEmails(taskRequestDTO.participants.toList())
        val task = Task(
            title = taskRequestDTO.title,
            description = taskRequestDTO.description,
            status = taskRequestDTO.status,
            createdBy = createdBy,
            participants = taskRequestDTO.participants
        )
        val savedTask = taskRepository.save(task)
        logger.info("New task created with ID: ${savedTask.id}")
        return savedTask
    }

    fun addParticipantToTask(taskId: Long, participantIds: List<String>, addedBy: String): Task {
        logger.info("Adding participants to task ID: $taskId by user: $addedBy, participants: ${participantIds.joinToString(", ")}")
        val task = taskRepository.findById(taskId).orElseThrow { RuntimeException("Task not found") }
        verifyUserIsParticipant(taskId, addedBy)
        validateEmails(participantIds)

        participantIds.forEach { task.participants.add(it) }
        val updatedTask = taskRepository.save(task)
        logger.info("Participants added successfully to task ID: $taskId")
        return updatedTask
    }

    private fun validateEmails(emails: List<String>) {
        logger.info("Validating emails: ${emails.joinToString(", ")}")
        val emailValidationResponse = emailValidatorService.sendAndReceive(emails).get(10, TimeUnit.SECONDS)

        if (emailValidationResponse.missingEmails.isNotEmpty()) {
            logger.error("Some emails are not registered: ${emailValidationResponse.missingEmails.joinToString(", ")}")
            throw RuntimeException("Some emails are not registered: ${emailValidationResponse.missingEmails.joinToString(", ")}")
        }
        logger.info("All emails are valid.")
    }

    fun updateTaskStatus(taskId: Long, newStatus: String, changedBy: String): Task {
        logger.info("Updating status of task ID: $taskId to $newStatus by user: $changedBy")
        val task = taskRepository.findById(taskId).orElseThrow { RuntimeException("Task not found") }
        verifyUserIsParticipant(taskId, changedBy)

        val previousStatus = task.status
        task.status = newStatus
        addTaskHistory(task, ActionType.STATUS_CHANGE, previousStatus, newStatus, changedBy)
        val updatedTask = taskRepository.save(task)
        logger.info("Task ID: $taskId status updated to $newStatus")
        return updatedTask
    }

    fun updateTaskDescription(taskId: Long, newDescription: String, changedBy: String): Task {
        logger.info("Updating description of task ID: $taskId by user: $changedBy")
        val task = taskRepository.findById(taskId).orElseThrow { RuntimeException("Task not found") }
        verifyUserIsParticipant(taskId, changedBy)

        val previousDescription = task.description
        task.description = newDescription
        addTaskHistory(task, ActionType.DESCRIPTION_CHANGE, previousDescription, newDescription, changedBy)
        val updatedTask = taskRepository.save(task)
        logger.info("Task ID: $taskId description updated")
        return updatedTask
    }

    fun updateTaskTitle(taskId: Long, newTitle: String, changedBy: String): Task {
        logger.info("Updating title of task ID: $taskId to $newTitle by user: $changedBy")
        val task = taskRepository.findById(taskId).orElseThrow { RuntimeException("Task not found") }
        verifyUserIsParticipant(taskId, changedBy)

        val previousTitle = task.title
        task.title = newTitle
        addTaskHistory(task, ActionType.TITLE_CHANGE, previousTitle, newTitle, changedBy)
        val updatedTask = taskRepository.save(task)
        logger.info("Task ID: $taskId title updated to $newTitle")
        return updatedTask
    }

    private fun verifyUserIsParticipant(taskId: Long, userId: String) {
        logger.info("Verifying if user: $userId is a participant in task ID: $taskId")
        val task = taskRepository.findById(taskId).orElseThrow { RuntimeException("Task not found") }
        if (task.participants.none { it == userId }) {
            logger.error("User: $userId is not a participant in task ID: $taskId")
            throw AccessDeniedException("User is not a participant of this task")
        }
        logger.info("User: $userId is a participant in task ID: $taskId")
    }

    private fun addTaskHistory(task: Task, action: ActionType, previousValue: String, newValue: String, changedBy: String) {
        logger.info("Adding history entry for task ID: ${task.id}, action: $action by user: $changedBy")
        val historyEntry = TaskHistory(
            task = task,
            action = action,
            previousValue = previousValue,
            newValue = newValue,
            changedBy = changedBy
        )
        taskHistoryRepository.save(historyEntry)
        logger.info("History entry added successfully for task ID: ${task.id}")
    }
}
