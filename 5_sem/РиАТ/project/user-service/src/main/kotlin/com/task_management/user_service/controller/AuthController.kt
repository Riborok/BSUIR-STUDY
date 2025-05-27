package com.task_management.user_service.controller

import com.task_management.user_service.dto.JwtResponse
import com.task_management.user_service.dto.LoginRequest
import com.task_management.user_service.dto.SignupRequest
import com.task_management.user_service.jwt.JwtUtils
import com.task_management.user_service.service.AuthenticationService
import com.task_management.user_service.service.UserService
import jakarta.servlet.http.HttpServletRequest
import org.springframework.http.HttpStatus
import org.springframework.http.ResponseEntity
import org.springframework.web.bind.annotation.*

@RestController
@RequestMapping("/api/auth")
class AuthController(
    private val userService: UserService,
    private val jwtUtils: JwtUtils,
    private val authenticationService: AuthenticationService
) {
    @GetMapping("/validate-token")
    fun validateToken(request: HttpServletRequest): ResponseEntity<Unit> {
        val jwt = jwtUtils.parseJwt(request)
        return if (jwt != null && jwtUtils.validateJwtToken(jwt)) {
            ResponseEntity.ok().build()
        } else {
            ResponseEntity.status(HttpStatus.UNAUTHORIZED).build()
        }
    }

    @PostMapping("/signin")
    fun authenticateUser(@RequestBody loginRequest: LoginRequest): ResponseEntity<JwtResponse> {
        val jwt = authenticationService.authenticateAndGenerateJwt(loginRequest)
        return ResponseEntity.ok(
            JwtResponse(
                token = jwt
            )
        )
    }

    @PostMapping("/signup")
    fun registerUser(@RequestBody signUpRequest: SignupRequest): ResponseEntity<JwtResponse> {
        val user = userService.registerUser(
            email = signUpRequest.email,
            password = signUpRequest.password,
            firstName = signUpRequest.firstName,
            lastName = signUpRequest.lastName
        )
        val jwt = authenticationService.authenticateAndGenerateJwt(signUpRequest)
        return ResponseEntity.ok(
            JwtResponse(
                token = jwt
            )
        )
    }
}