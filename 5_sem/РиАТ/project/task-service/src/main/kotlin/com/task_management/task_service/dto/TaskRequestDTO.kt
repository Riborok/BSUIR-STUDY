package com.task_management.task_service.dto

data class TaskRequestDTO(
    val title: String,
    val description: String,
    val status: String,
    val participants: MutableSet<String>
)