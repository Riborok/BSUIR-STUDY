package com.task_management.task_service.controller

import com.task_management.task_service.dto.TaskHistoryResponseDTO
import com.task_management.task_service.dto.TaskRequestDTO
import com.task_management.task_service.dto.TaskResponseDTO
import com.task_management.task_service.kafka.EmailNotificationService
import com.task_management.task_service.service.TaskService
import org.springframework.http.HttpStatus
import org.springframework.http.ResponseEntity
import org.springframework.security.core.annotation.AuthenticationPrincipal
import org.springframework.web.bind.annotation.*
import java.security.Principal

@RestController
@RequestMapping("/api/tasks")
class TaskController(
    private val taskService: TaskService,
    private val emailNotificationService: EmailNotificationService
) {
    @GetMapping
    fun getAllTasksForUser(@AuthenticationPrincipal principal: Principal): ResponseEntity<List<TaskResponseDTO>> {
        val userId = principal.name

        val response = taskService.getAllTasksForUser(userId).map { task ->
            TaskResponseDTO(
                id = task.id,
                title = task.title,
                description = task.description,
                status = task.status,
                createdBy = task.createdBy,
                participants = task.participants.map { it }.toMutableSet()
            )
        }
        return ResponseEntity.ok(response)
    }

    @GetMapping("/{taskId}/history")
    fun getTaskHistory(@AuthenticationPrincipal principal: Principal, @PathVariable taskId: Long): ResponseEntity<List<TaskHistoryResponseDTO>> {
        val userId = principal.name

        val history = taskService.getTaskHistory(taskId, userId).map { historyEntry ->
            TaskHistoryResponseDTO(
                action = historyEntry.action,
                previousValue = historyEntry.previousValue,
                newValue = historyEntry.newValue,
                changedBy = historyEntry.changedBy,
                changedAt = historyEntry.changedAt
            )
        }
        return ResponseEntity.ok(history)
    }

    @PostMapping
    fun addTask(@AuthenticationPrincipal principal: Principal, @RequestBody taskRequestDTO: TaskRequestDTO): ResponseEntity<TaskResponseDTO> {
        val userId = principal.name

        val task = taskService.addTask(taskRequestDTO, userId)
        emailNotificationService.sendTaskCreationMessage(task, userId)
        val response = TaskResponseDTO(
            id = task.id,
            title = task.title,
            description = task.description,
            status = task.status,
            createdBy = task.createdBy,
            participants = task.participants.map { it }.toMutableSet()
        )
        return ResponseEntity.status(HttpStatus.CREATED).body(response)
    }

    @DeleteMapping("/{taskId}")
    fun deleteTask(@AuthenticationPrincipal principal: Principal, @PathVariable taskId: Long): ResponseEntity<Void> {
        val userId = principal.name

        val task = taskService.deleteTask(taskId, userId)
        emailNotificationService.sendTaskDeletionMessage(task, userId)
        return ResponseEntity.noContent().build()
    }

    @PutMapping("/{taskId}/participants")
    fun addParticipantToTask(
        @AuthenticationPrincipal principal: Principal,
        @PathVariable taskId: Long,
        @RequestBody participantIds: List<String>
    ): ResponseEntity<Void> {
        val userId = principal.name

        val task = taskService.addParticipantToTask(taskId, participantIds, userId)
        emailNotificationService.sendParticipantAddedMessage(task, userId, participantIds)
        return ResponseEntity.noContent().build()
    }

    @PutMapping("/{taskId}/status")
    fun updateTaskStatus(
        @AuthenticationPrincipal principal: Principal,
        @PathVariable taskId: Long,
        @RequestParam newStatus: String
    ): ResponseEntity<Void> {
        val userId = principal.name

        val task = taskService.updateTaskStatus(taskId, newStatus, userId)
        emailNotificationService.sendTaskStatusUpdateMessage(task, userId)
        return ResponseEntity.noContent().build()
    }

    @PutMapping("/{taskId}/description")
    fun updateTaskDescription(
        @AuthenticationPrincipal principal: Principal,
        @PathVariable taskId: Long,
        @RequestParam newDescription: String
    ): ResponseEntity<Void> {
        val userId = principal.name

        val task = taskService.updateTaskDescription(taskId, newDescription, userId)
        emailNotificationService.sendTaskDescriptionUpdateMessage(task, userId)
        return ResponseEntity.noContent().build()
    }

    @PutMapping("/{taskId}/title")
    fun updateTaskTitle(
        @AuthenticationPrincipal principal: Principal,
        @PathVariable taskId: Long,
        @RequestParam newTitle: String
    ): ResponseEntity<Void> {
        val userId = principal.name

        val task = taskService.updateTaskTitle(taskId, newTitle, userId)
        emailNotificationService.sendTaskTitleUpdateMessage(task, userId)
        return ResponseEntity.noContent().build()
    }
}
