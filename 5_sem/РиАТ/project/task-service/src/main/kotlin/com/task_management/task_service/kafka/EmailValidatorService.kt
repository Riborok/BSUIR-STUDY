package com.task_management.task_service.kafka

import org.springframework.kafka.annotation.KafkaListener
import org.springframework.kafka.core.KafkaTemplate
import org.springframework.stereotype.Service
import java.util.concurrent.CompletableFuture
import java.util.concurrent.ConcurrentHashMap
import org.slf4j.LoggerFactory

data class EmailValidationResponse(
    val emails: List<String>,
    val missingEmails: List<String>
)

@Service
class EmailValidatorService(
    private val kafkaTemplate: KafkaTemplate<String, List<String>>,
) {
    private val logger = LoggerFactory.getLogger(EmailValidatorService::class.java)
    private val responseMap = ConcurrentHashMap<String, CompletableFuture<EmailValidationResponse>>()

    fun sendAndReceive(emails: List<String>): CompletableFuture<EmailValidationResponse> {
        val key = emails.joinToString(", ")
        logger.info("Sending email validation request for emails: $key")
        val future = CompletableFuture<EmailValidationResponse>()
        responseMap[key] = future
        kafkaTemplate.send("email-validation-request", emails)
        logger.info("Sent email validation request for emails: $key")
        return future
    }

    @KafkaListener(topics = ["email-validation-response"], groupId = "task-service")
    fun listenValidationResult(response: EmailValidationResponse) {
        val key = response.emails.joinToString(", ")
        logger.info("Received email validation response for emails: $key")
        responseMap[key]?.complete(response)
        responseMap.remove(key)
        logger.info("Completed email validation for emails: $key")
    }
}
