package com.task_management.task_service.jwt

import jakarta.servlet.http.HttpServletRequest
import org.slf4j.LoggerFactory
import org.springframework.stereotype.Component
import org.springframework.util.StringUtils

@Component
class JwtUtils {
    private val logger = LoggerFactory.getLogger(JwtUtils::class.java)

    fun parseJwt(request: HttpServletRequest): String? {
        val headerAuth = request.getHeader("Authorization")
        logger.info("Parsing JWT token from Authorization header in request: ${request.requestURI}")

        if (StringUtils.hasText(headerAuth) && headerAuth.startsWith("Bearer ")) {
            val token = headerAuth.substring("Bearer ".length)
            logger.info("JWT token found in request: ${request.requestURI}, token: ${token.take(30)}...")
            return token
        }

        logger.warn("Authorization header is missing or does not contain Bearer token in request: ${request.requestURI}")
        return null
    }
}