package com.task_management.user_service.service

import com.task_management.user_service.jwt.JwtUtils
import org.springframework.security.authentication.AuthenticationManager
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken
import org.springframework.security.core.context.SecurityContextHolder
import org.springframework.stereotype.Service

open class Credentials(
    open val email: String,
    open val password: String
)

@Service
class AuthenticationService(
    private val authenticationManager: AuthenticationManager,
    private val jwtUtils: JwtUtils
) {
    fun authenticateAndGenerateJwt(credentials: Credentials): String {
        val authentication = authenticationManager.authenticate(
            UsernamePasswordAuthenticationToken(credentials.email, credentials.password)
        )
        SecurityContextHolder.getContext().authentication = authentication
        return jwtUtils.generateJwtToken(authentication)
    }
}
