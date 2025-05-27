package com.task_management.task_service.jwt

import com.task_management.task_service.kafka.JwtValidatorService
import jakarta.servlet.FilterChain
import jakarta.servlet.http.HttpServletRequest
import jakarta.servlet.http.HttpServletResponse
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken
import org.springframework.security.core.authority.SimpleGrantedAuthority
import org.springframework.security.core.context.SecurityContextHolder
import org.springframework.stereotype.Component
import org.springframework.web.filter.OncePerRequestFilter
import java.security.Principal
import java.util.concurrent.TimeUnit
import org.slf4j.LoggerFactory

@Component
class AuthTokenFilter(
    private val jwtUtils: JwtUtils,
    private val jwtValidator: JwtValidatorService
) : OncePerRequestFilter() {
    private val authLogger = LoggerFactory.getLogger(AuthTokenFilter::class.java)

    override fun doFilterInternal(
        request: HttpServletRequest,
        response: HttpServletResponse,
        filterChain: FilterChain
    ) {
        try {
            val jwt = jwtUtils.parseJwt(request)

            if (jwt != null) {
                authLogger.info("Found JWT token in request: ${jwt.take(30)}...")

                authLogger.info("Validating JWT token...")
                val result = jwtValidator.sendAndReceive(jwt).get(10, TimeUnit.SECONDS)

                if (result.claims != null) {
                    authLogger.info("JWT validation successful, claims extracted.")

                    val claims = result.claims

                    val username: String = claims["sub"].toString()
                    val roles = claims["roles"].toString().split(",").map { SimpleGrantedAuthority(it) }

                    authLogger.info("Extracted username: $username, roles: ${roles.joinToString(", ")}")

                    val authentication = UsernamePasswordAuthenticationToken(
                        Principal { username },
                        null,
                        roles
                    ).apply {
                        details = mapOf(
                            "firstName" to claims["firstName"].toString(),
                            "lastName" to claims["lastName"].toString()
                        )
                    }

                    SecurityContextHolder.getContext().authentication = authentication
                    authLogger.info("Authentication set for user: $username")
                } else {
                    authLogger.warn("JWT token validation failed: Claims are null.")
                }
            } else {
                authLogger.warn("No JWT token found in request.")
            }

        } catch (e: Exception) {
            authLogger.error("Error during JWT validation or authentication process: {}", e.message, e)
        }

        filterChain.doFilter(request, response)
    }
}