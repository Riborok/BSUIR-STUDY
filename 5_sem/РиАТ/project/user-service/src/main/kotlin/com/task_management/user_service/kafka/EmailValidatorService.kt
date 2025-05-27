package com.task_management.user_service.kafka

import com.task_management.user_service.repository.UserRepository
import org.springframework.kafka.annotation.KafkaListener
import org.springframework.kafka.core.KafkaTemplate
import org.springframework.stereotype.Service
import org.slf4j.LoggerFactory

data class EmailValidationResponse(
    val emails: List<String>,
    val missingEmails: List<String>
)

@Service
class EmailValidatorService(
    private val kafkaTemplate: KafkaTemplate<String, EmailValidationResponse>,
    private val userRepository: UserRepository,
) {
    private val logger = LoggerFactory.getLogger(EmailValidatorService::class.java)

    @KafkaListener(topics = ["email-validation-request"], groupId = "user-service")
    fun validateEmails(emails: List<String>) {
        logger.info("Received email validation request for emails: ${emails.joinToString(", ")}")

        val existingEmails = userRepository.findAllByEmailIn(emails).map { it.email }
        logger.info("Existing emails found in the database: ${existingEmails.joinToString(", ")}")

        val missingEmails = emails.filter { it !in existingEmails }
        logger.info("Missing emails: ${missingEmails.joinToString(", ")}")

        val response = EmailValidationResponse(
            emails = emails,
            missingEmails = missingEmails
        )

        logger.info("Sending email validation response to Kafka, missing emails: ${missingEmails.joinToString(", ")}")
        kafkaTemplate.send("email-validation-response", response)
        logger.info("Email validation response sent successfully, missing emails: ${missingEmails.joinToString(", ")}")
    }
}
