package com.task_management.task_service.dto

import com.task_management.task_service.entity.ActionType
import java.time.LocalDateTime

data class TaskHistoryResponseDTO(
    val action: ActionType,
    val previousValue: String,
    val newValue: String,
    val changedBy: String,
    val changedAt: LocalDateTime
)