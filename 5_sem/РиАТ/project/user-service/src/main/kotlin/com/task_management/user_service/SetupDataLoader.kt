package com.task_management.user_service

import com.task_management.user_service.entity.ERole
import com.task_management.user_service.entity.Role
import com.task_management.user_service.repository.RoleRepository
import org.springframework.boot.context.event.ApplicationReadyEvent
import org.springframework.context.event.EventListener
import org.springframework.stereotype.Component

@Component
class SetupDataLoader(
    private val roleRepository: RoleRepository
) {

    @EventListener
    fun onApplicationReady(event: ApplicationReadyEvent) {
        if (!roleRepository.findByName(ERole.ROLE_USER).isPresent) {
            roleRepository.save(Role(name = ERole.ROLE_USER))
        }
    }
}