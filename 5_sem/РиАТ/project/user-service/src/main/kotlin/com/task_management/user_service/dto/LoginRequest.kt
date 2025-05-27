package com.task_management.user_service.dto

import com.task_management.user_service.service.Credentials

data class LoginRequest(
    override val email: String,
    override val password: String
) : Credentials(email, password)