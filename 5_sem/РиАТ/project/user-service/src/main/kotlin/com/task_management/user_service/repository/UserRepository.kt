package com.task_management.user_service.repository

import com.task_management.user_service.entity.User
import org.springframework.data.jpa.repository.JpaRepository
import org.springframework.stereotype.Repository
import java.util.*

@Repository
interface UserRepository : JpaRepository<User, Long> {
    fun findByEmail(email: String): Optional<User>
    fun findAllByEmailIn(emails: List<String>): List<User>
}