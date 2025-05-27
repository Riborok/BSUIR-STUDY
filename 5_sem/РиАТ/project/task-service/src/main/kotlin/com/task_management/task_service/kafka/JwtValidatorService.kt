package com.task_management.task_service.kafka

import org.springframework.kafka.annotation.KafkaListener
import org.springframework.kafka.core.KafkaTemplate
import org.springframework.stereotype.Service
import java.util.concurrent.CompletableFuture
import java.util.concurrent.ConcurrentHashMap
import org.slf4j.LoggerFactory

data class JwtValidationResponse(
    val token: String,
    val claims: Map<String, Any>?
)

@Service
class JwtValidatorService(
    private val kafkaTemplate: KafkaTemplate<String, String>
) {
    private val logger = LoggerFactory.getLogger(JwtValidatorService::class.java)
    private val responseMap = ConcurrentHashMap<String, CompletableFuture<JwtValidationResponse>>()

    fun sendAndReceive(token: String): CompletableFuture<JwtValidationResponse> {
        logger.info("Sending JWT token for validation: ${token.take(30)}...")
        val future = CompletableFuture<JwtValidationResponse>()
        responseMap[token] = future
        kafkaTemplate.send("jwt-validation-request", token)
        logger.info("Sent JWT token for validation: ${token.take(30)}...")
        return future
    }

    @KafkaListener(topics = ["jwt-validation-response"], groupId = "task-service")
    fun listenValidationResult(message: JwtValidationResponse) {
        logger.info("Received JWT validation response for token: ${message.token.take(30)}...")
        responseMap[message.token]?.complete(message)
        responseMap.remove(message.token)
        logger.info("Completed validation for token: ${message.token.take(30)}...")
    }
}
