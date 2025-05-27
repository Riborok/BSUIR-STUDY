package com.task_management.task_service.repository

import com.task_management.task_service.entity.TaskHistory
import org.springframework.data.jpa.repository.JpaRepository

interface TaskHistoryRepository : JpaRepository<TaskHistory, Long>