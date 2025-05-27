package com.task_management.user_service.controller

import org.springframework.http.ResponseEntity
import org.springframework.web.bind.annotation.ControllerAdvice
import org.springframework.web.bind.annotation.ExceptionHandler

@ControllerAdvice
class RestStatusExceptionHandler {
    @ExceptionHandler(Exception::class)
    fun handleRestStatusException(ex: Exception): ResponseEntity<String> {
        return ResponseEntity.badRequest().body(ex.message)
    }
}