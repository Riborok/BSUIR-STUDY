package com.task_management.user_service.jwt

import jakarta.servlet.FilterChain
import jakarta.servlet.http.HttpServletRequest
import jakarta.servlet.http.HttpServletResponse
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken
import org.springframework.security.core.context.SecurityContextHolder
import org.springframework.security.web.authentication.WebAuthenticationDetailsSource
import org.springframework.stereotype.Component
import org.slf4j.LoggerFactory
import org.springframework.web.filter.OncePerRequestFilter

@Component
class AuthTokenFilter(
    private val jwtUtils: JwtUtils,
    private val userDetailsService: UserDetailsServiceImpl
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

                if (jwtUtils.validateJwtToken(jwt)) {
                    authLogger.info("JWT token is valid")

                    val username = jwtUtils.getUserNameFromJwtToken(jwt)
                    authLogger.info("Username extracted from JWT: $username")

                    val userDetails = userDetailsService.loadUserByUsername(username)
                    val authentication = UsernamePasswordAuthenticationToken(
                        userDetails, null, userDetails.authorities
                    )
                    authentication.details = WebAuthenticationDetailsSource().buildDetails(request)
                    SecurityContextHolder.getContext().authentication = authentication
                    authLogger.info("Authentication set for user: $username")
                } else {
                    authLogger.warn("Invalid JWT token detected")
                }
            } else {
                authLogger.warn("No JWT token found in request")
            }
        } catch (e: Exception) {
            authLogger.error("Cannot set user authentication:", e)
        }

        filterChain.doFilter(request, response)
    }
}