package com.task_management.task_service.kafka

import com.task_management.task_service.entity.Task
import org.springframework.kafka.core.KafkaTemplate
import org.springframework.stereotype.Service
import org.slf4j.LoggerFactory

data class TaskRequestDTO(
    val title: String,
    val description: String,
    val status: String,
    val participants: MutableSet<String>,
    val actionByUserId: String
) {
    constructor(task: Task, userId: String) : this(
        title = task.title,
        description = task.description,
        status = task.status,
        participants = task.participants.toMutableSet(),
        userId
    )
}

data class ParticipantAddedMessage(
    val taskRequestDTO: TaskRequestDTO,
    val newParticipants: List<String>
)

@Service
class EmailNotificationService(
    private val kafkaTemplate: KafkaTemplate<String, Any>
) {
    private val logger = LoggerFactory.getLogger(EmailNotificationService::class.java)

    fun sendTaskCreationMessage(task: Task, userId: String) {
        val message = TaskRequestDTO(task, userId)
        logger.info("Sending task creation message for taskId: ${task.id} to userId: $userId")
        kafkaTemplate.send("task-created", message)
        logger.info("Sent task creation message for taskId: ${task.id} to userId: $userId")
    }

    fun sendTaskDeletionMessage(task: Task, userId: String) {
        val message = TaskRequestDTO(task, userId)
        logger.info("Sending task deletion message for taskId: ${task.id} to userId: $userId")
        kafkaTemplate.send("task-deleted", message)
        logger.info("Sent task deletion message for taskId: ${task.id} to userId: $userId")
    }

    fun sendParticipantAddedMessage(task: Task, userId: String, newParticipants: List<String>) {
        val message = ParticipantAddedMessage(TaskRequestDTO(task, userId), newParticipants)
        logger.info("Sending participant added message for taskId: ${task.id} to userId: $userId with new participants: ${newParticipants.joinToString(", ")}")
        kafkaTemplate.send("participant-added", message)
        logger.info("Sent participant added message for taskId: ${task.id} to userId: $userId with new participants: ${newParticipants.joinToString(", ")}")
    }

    fun sendTaskStatusUpdateMessage(task: Task, userId: String) {
        val message = TaskRequestDTO(task, userId)
        logger.info("Sending task status update message for taskId: ${task.id} to userId: $userId")
        kafkaTemplate.send("task-status-updated", message)
        logger.info("Sent task status update message for taskId: ${task.id} to userId: $userId")
    }

    fun sendTaskDescriptionUpdateMessage(task: Task, userId: String) {
        val message = TaskRequestDTO(task, userId)
        logger.info("Sending task description update message for taskId: ${task.id} to userId: $userId")
        kafkaTemplate.send("task-description-updated", message)
        logger.info("Sent task description update message for taskId: ${task.id} to userId: $userId")
    }

    fun sendTaskTitleUpdateMessage(task: Task, userId: String) {
        val message = TaskRequestDTO(task, userId)
        logger.info("Sending task title update message for taskId: ${task.id} to userId: $userId")
        kafkaTemplate.send("task-title-updated", message)
        logger.info("Sent task title update message for taskId: ${task.id} to userId: $userId")
    }
}
