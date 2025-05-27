package com.task_management.task_service.dto

data class TaskResponseDTO(
    val id: Long,
    val title: String,
    val description: String,
    val status: String,
    val createdBy: String,
    val participants: MutableSet<String>
)