package com.task_management.task_service.repository

import com.task_management.task_service.entity.Task
import org.springframework.data.jpa.repository.JpaRepository
import org.springframework.data.jpa.repository.Query
import org.springframework.data.repository.query.Param

interface TaskRepository : JpaRepository<Task, Long> {
    @Query("""
        SELECT t FROM Task t
        JOIN t.participants p
        WHERE p = :userId
    """)
    fun findAllTasksForUser(@Param("userId") userId: String): List<Task>
}