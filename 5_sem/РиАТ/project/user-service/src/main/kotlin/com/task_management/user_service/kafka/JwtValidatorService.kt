package com.task_management.user_service.kafka

import com.task_management.user_service.jwt.JwtUtils
import org.springframework.kafka.annotation.KafkaListener
import org.springframework.kafka.core.KafkaTemplate
import org.springframework.stereotype.Service
import org.slf4j.LoggerFactory

data class JwtValidationResponse(
    val token: String,
    val claims: Map<String, Any>?
)

@Service
class JwtValidatorService(
    private val kafkaTemplate: KafkaTemplate<String, JwtValidationResponse>,
    private val jwtUtils: JwtUtils
) {
    private val logger = LoggerFactory.getLogger(JwtValidatorService::class.java)

    @KafkaListener(topics = ["jwt-validation-request"], groupId = "user-service")
    fun validateToken(token: String) {
        logger.info("Received JWT validation request for token: ${token.take(30)}...")

        val isValid = jwtUtils.validateJwtToken(token)
        if (isValid) {
            logger.info("JWT token is valid: ${token.take(30)}...")
        } else {
            logger.error("Invalid JWT token: ${token.take(30)}...")
        }

        val claims = if (isValid) {
            jwtUtils.getClaimsFromJwt(token)
        } else {
            null
        }

        val response = JwtValidationResponse(token, claims)
        logger.info("Sending JWT validation response to Kafka for token: ${token.take(30)}...")
        kafkaTemplate.send("jwt-validation-response", response)
        logger.info("JWT validation response sent successfully for token: ${token.take(30)}...")
    }
}