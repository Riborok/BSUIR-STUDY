package com.task_management.user_service.dto

data class JwtResponse(
    val token: String,
    val type: String = "Bearer"
)