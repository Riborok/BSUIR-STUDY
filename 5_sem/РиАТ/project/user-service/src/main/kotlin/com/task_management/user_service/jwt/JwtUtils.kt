package com.task_management.user_service.jwt

import io.jsonwebtoken.*
import jakarta.servlet.http.HttpServletRequest
import org.springframework.beans.factory.annotation.Value
import org.springframework.security.core.Authentication
import org.springframework.stereotype.Component
import org.springframework.util.StringUtils
import java.util.*
import org.slf4j.LoggerFactory

@Component
class JwtUtils(
    @Value("\${app.jwtSecret}") private val jwtSecret: String,
    @Value("\${app.jwtExpirationMs}") private val jwtExpirationMs: Int
) {
    private val logger = LoggerFactory.getLogger(JwtUtils::class.java)

    fun generateJwtToken(authentication: Authentication): String {
        val userPrincipal = authentication.principal as UserDetailsImpl
        val roles = userPrincipal.authorities.joinToString(",") { it.authority }

        logger.info("Generating JWT token for user: ${userPrincipal.username}, with roles: $roles")

        val token = Jwts.builder()
            .setSubject(userPrincipal.username)
            .setIssuedAt(Date())
            .setExpiration(Date(Date().time + jwtExpirationMs))
            .claim("firstName", userPrincipal.getFirstName())
            .claim("lastName", userPrincipal.getLastName())
            .claim("roles", roles)
            .signWith(SignatureAlgorithm.HS512, jwtSecret)
            .compact()

        logger.info("JWT token successfully generated for user: ${userPrincipal.username}, token: ${token.take(30)}...")
        return token
    }

    fun getUserNameFromJwtToken(token: String): String {
        logger.info("Extracting username from JWT token: ${token.take(30)}...")

        return try {
            val username = Jwts.parser()
                .setSigningKey(jwtSecret)
                .parseClaimsJws(token)
                .body
                .subject

            logger.info("Username extracted from JWT token: $username")
            username
        } catch (e: Exception) {
            logger.error("Error extracting username from JWT token: ${token.take(30)}...", e)
            throw RuntimeException("Failed to extract username from JWT token", e)
        }
    }

    fun getClaimsFromJwt(token: String): Claims {
        logger.info("Extracting claims from JWT token: ${token.take(30)}...")

        return try {
            val claims = Jwts.parser()
                .setSigningKey(Base64.getDecoder().decode(jwtSecret))
                .parseClaimsJws(token)
                .body

            logger.info("Claims successfully extracted from JWT token: ${claims.toString().take(50)}...")
            claims
        } catch (e: Exception) {
            logger.error("Error extracting claims from JWT token: ${token.take(30)}...", e)
            throw RuntimeException("Failed to extract claims from JWT token", e)
        }
    }

    fun validateJwtToken(authToken: String): Boolean {
        logger.info("Validating JWT token: ${authToken.take(30)}...")

        try {
            Jwts.parser().setSigningKey(jwtSecret).parseClaimsJws(authToken)
            logger.info("JWT token is valid: ${authToken.take(30)}...")
            return true
        } catch (e: SignatureException) {
            logger.error("Invalid JWT signature: ${authToken.take(30)}...", e)
        } catch (e: MalformedJwtException) {
            logger.error("Invalid JWT token: ${authToken.take(30)}...", e)
        } catch (e: ExpiredJwtException) {
            logger.error("JWT token is expired: ${authToken.take(30)}...", e)
        } catch (e: UnsupportedJwtException) {
            logger.error("JWT token is unsupported: ${authToken.take(30)}...", e)
        } catch (e: IllegalArgumentException) {
            logger.error("JWT claims string is empty: ${authToken.take(30)}...", e)
        } catch (e: Exception) {
            logger.error("Error during JWT token validation: ${authToken.take(30)}...", e)
        }
        return false
    }

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
