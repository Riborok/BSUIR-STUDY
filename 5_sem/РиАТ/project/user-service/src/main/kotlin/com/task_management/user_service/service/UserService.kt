package com.task_management.user_service.service

import com.task_management.user_service.entity.ERole
import com.task_management.user_service.entity.Profile
import com.task_management.user_service.entity.User
import com.task_management.user_service.repository.RoleRepository
import com.task_management.user_service.repository.UserRepository
import org.springframework.dao.DataIntegrityViolationException
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder
import org.springframework.stereotype.Service
import org.springframework.transaction.annotation.Transactional
import org.slf4j.LoggerFactory

@Service
@Transactional
class UserService(
    private val userRepository: UserRepository,
    private val roleRepository: RoleRepository
) {
    private val BCryptPasswordEncoder = BCryptPasswordEncoder()
    private val logger = LoggerFactory.getLogger(UserService::class.java)

    fun registerUser(email: String, password: String, firstName: String, lastName: String): User {
        logger.info("Attempting to register user with email: $email")

        val user = User(
            email = email,
            password = BCryptPasswordEncoder.encode(password),
            roles = mutableListOf(
                roleRepository.findByName(ERole.ROLE_USER)
                    .orElseThrow { RuntimeException("Role is not found.") }
            ),
            profile = Profile(
                firstName = firstName,
                lastName = lastName
            )
        ).also { it.profile.user = it }

        return try {
            logger.info("User with email: $email successfully created.")
            userRepository.save(user)
        } catch (ex: DataIntegrityViolationException) {
            logger.error("Error during user registration: Email already in use ($email)", ex)
            throw RuntimeException("Email is already in use!", ex)
        } catch (ex: Exception) {
            logger.error("Unexpected error occurred while registering user with email: $email", ex)
            throw RuntimeException("Error during user registration: An unexpected error occurred", ex)
        }
    }
}