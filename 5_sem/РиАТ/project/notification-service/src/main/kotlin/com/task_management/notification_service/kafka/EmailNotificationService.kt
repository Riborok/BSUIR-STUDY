package com.task_management.notification_service.kafka

import com.task_management.notification_service.service.NotificationService
import org.springframework.kafka.annotation.KafkaListener
import org.springframework.stereotype.Service

data class TaskRequestDTO(
    val title: String,
    val description: String,
    val status: String,
    val participants: MutableSet<String>,
    val actionByUserId: String
)

data class ParticipantAddedMessage(
    val taskRequestDTO: TaskRequestDTO,
    val newParticipants: List<String>
)

@Service
class EmailNotificationService(
    private val notificationService: NotificationService
) {
    @KafkaListener(topics = ["task-created"], groupId = "notification-service")
    fun notifyTaskCreation(taskRequestDTO: TaskRequestDTO) {
        notificationService.notifyTaskCreation(taskRequestDTO)
    }

    @KafkaListener(topics = ["task-deleted"], groupId = "notification-service")
    fun notifyTaskDeletion(taskRequestDTO: TaskRequestDTO) {
        notificationService.notifyTaskDeletion(taskRequestDTO)
    }

    @KafkaListener(topics = ["participant-added"], groupId = "notification-service")
    fun notifyParticipantAdded(message: ParticipantAddedMessage) {
        val (taskRequestDTO, newParticipants) = message
        notificationService.notifyParticipantsAdded(taskRequestDTO, newParticipants)
    }

    @KafkaListener(topics = ["task-status-updated"], groupId = "notification-service")
    fun notifyTaskStatusUpdate(message: TaskRequestDTO) {
        notificationService.notifyTaskStatusUpdate(message)
    }

    @KafkaListener(topics = ["task-description-updated"], groupId = "notification-service")
    fun notifyTaskDescriptionUpdate(message: TaskRequestDTO) {
        notificationService.notifyTaskDescriptionUpdate(message)
    }

    @KafkaListener(topics = ["task-title-updated"], groupId = "notification-service")
    fun notifyTaskTitleUpdate(message: TaskRequestDTO) {
        notificationService.notifyTaskTitleUpdate(message)
    }
}
