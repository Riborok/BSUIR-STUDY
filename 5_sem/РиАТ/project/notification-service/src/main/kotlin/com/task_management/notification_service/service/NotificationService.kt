package com.task_management.notification_service.service

import com.task_management.notification_service.kafka.TaskRequestDTO
import org.springframework.stereotype.Service
import org.slf4j.LoggerFactory

@Service
class NotificationService(
    private val emailService: EmailService
) {
    private val logger = LoggerFactory.getLogger(NotificationService::class.java)

    fun notifyTaskCreation(taskRequestDTO: TaskRequestDTO) {
        val message = """
            <p>Hello,</p>
            <p>A new task has been assigned to you: <strong>${taskRequestDTO.title}</strong></p>
            <p>Description: ${taskRequestDTO.description}</p>
            <p>Status: ${taskRequestDTO.status}</p>
            <p>Participants: ${taskRequestDTO.participants.joinToString(", ")}</p>
            <p>Created by: ${taskRequestDTO.actionByUserId}</p>
            <p>Best regards,</p>
            <p>Task Management Notification Service</p>
        """.trimIndent()

        logger.info("Sending task creation notification for task: ${taskRequestDTO.title}, recipients: ${taskRequestDTO.participants.size} participants.")

        taskRequestDTO.participants.forEach { participantEmail ->
            emailService.sendEmail(
                to = participantEmail,
                subject = "New Task Assigned: ${taskRequestDTO.title}",
                body = message
            )
        }

        logger.info("Task creation notification sent successfully for task: ${taskRequestDTO.title}")
    }

    fun notifyTaskDeletion(taskRequestDTO: TaskRequestDTO) {
        val message = """
            <p>Hello,</p>
            <p>The task <strong>${taskRequestDTO.title}</strong> has been deleted.</p>
            <p>Deleted by: ${taskRequestDTO.actionByUserId}</p>
            <p>Best regards,</p>
            <p>Task Management Notification Service</p>
        """.trimIndent()

        logger.info("Sending task deletion notification for task: ${taskRequestDTO.title}, recipients: ${taskRequestDTO.participants.size} participants.")

        taskRequestDTO.participants.forEach { participantEmail ->
            emailService.sendEmail(
                to = participantEmail,
                subject = "Task Deleted: ${taskRequestDTO.title}",
                body = message
            )
        }

        logger.info("Task deletion notification sent successfully for task: ${taskRequestDTO.title}")
    }

    fun notifyParticipantsAdded(taskRequestDTO: TaskRequestDTO, newParticipants: List<String>) {
        val messageToAll = """
            <p>Hello,</p>
            <p>A new participant has joined the task: <strong>${taskRequestDTO.title}</strong></p>
            <p>New participants: ${newParticipants.joinToString(", ")}</p>
            <p>Added by: ${taskRequestDTO.actionByUserId}</p>
            <p>Best regards,</p>
            <p>Task Management Notification Service</p>
        """.trimIndent()

        logger.info("Sending participant added notification for task: ${taskRequestDTO.title}, new participants: ${newParticipants.joinToString(", ")}")

        taskRequestDTO.participants.filter { it !in newParticipants }.forEach { participantEmail ->
            emailService.sendEmail(
                to = participantEmail,
                subject = "New Participant in Task: ${taskRequestDTO.title}",
                body = messageToAll
            )
        }

        newParticipants.forEach { newParticipant ->
            val messageToNewParticipant = """
                <p>Hello,</p>
                <p>You have been added to a new task: <strong>${taskRequestDTO.title}</strong></p>
                <p>Description: ${taskRequestDTO.description}</p>
                <p>Status: ${taskRequestDTO.status}</p>
                <p>Participants: ${taskRequestDTO.participants.joinToString(", ")}</p>
                <p>Added by: ${taskRequestDTO.actionByUserId}</p>
                <p>Best regards,</p>
                <p>Task Management Notification Service</p>
            """.trimIndent()

            emailService.sendEmail(
                to = newParticipant,
                subject = "Added to Task: ${taskRequestDTO.title}",
                body = messageToNewParticipant
            )
        }

        logger.info("Participant added notification sent for task: ${taskRequestDTO.title}, new participants: ${newParticipants.joinToString(", ")}")
    }

    fun notifyTaskStatusUpdate(taskRequestDTO: TaskRequestDTO) {
        val message = """
            <p>Hello,</p>
            <p>The task <strong>${taskRequestDTO.title}</strong> status has been updated to: ${taskRequestDTO.status}.</p>
            <p>Updated by: ${taskRequestDTO.actionByUserId}</p>
            <p>Best regards,</p>
            <p>Task Management Notification Service</p>
        """.trimIndent()

        logger.info("Sending task status update notification for task: ${taskRequestDTO.title}, recipients: ${taskRequestDTO.participants.size} participants.")

        taskRequestDTO.participants.forEach { participantEmail ->
            emailService.sendEmail(
                to = participantEmail,
                subject = "Task Status Updated: ${taskRequestDTO.title}",
                body = message
            )
        }

        logger.info("Task status update notification sent successfully for task: ${taskRequestDTO.title}")
    }

    fun notifyTaskDescriptionUpdate(taskRequestDTO: TaskRequestDTO) {
        val message = """
            <p>Hello,</p>
            <p>The task <strong>${taskRequestDTO.title}</strong> description has been updated.</p>
            <p>Updated by: ${taskRequestDTO.actionByUserId}</p>
            <p>Description: ${taskRequestDTO.description}</p>
            <p>Best regards,</p>
            <p>Task Management Notification Service</p>
        """.trimIndent()

        logger.info("Sending task description update notification for task: ${taskRequestDTO.title}, recipients: ${taskRequestDTO.participants.size} participants.")

        taskRequestDTO.participants.forEach { participantEmail ->
            emailService.sendEmail(
                to = participantEmail,
                subject = "Task Description Updated: ${taskRequestDTO.title}",
                body = message
            )
        }

        logger.info("Task description update notification sent successfully for task: ${taskRequestDTO.title}")
    }

    fun notifyTaskTitleUpdate(taskRequestDTO: TaskRequestDTO) {
        val message = """
            <p>Hello,</p>
            <p>The task title has been updated to <strong>${taskRequestDTO.title}</strong>.</p>
            <p>Updated by: ${taskRequestDTO.actionByUserId}</p>
            <p>Best regards,</p>
            <p>Task Management Notification Service</p>
        """.trimIndent()

        logger.info("Sending task title update notification for task: ${taskRequestDTO.title}, recipients: ${taskRequestDTO.participants.size} participants.")

        taskRequestDTO.participants.forEach { participantEmail ->
            emailService.sendEmail(
                to = participantEmail,
                subject = "Task Title Updated: ${taskRequestDTO.title}",
                body = message
            )
        }

        logger.info("Task title update notification sent successfully for task: ${taskRequestDTO.title}")
    }
}
