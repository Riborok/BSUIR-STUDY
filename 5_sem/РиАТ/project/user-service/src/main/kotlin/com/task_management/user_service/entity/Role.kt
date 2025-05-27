package com.task_management.user_service.entity

import jakarta.persistence.*

@Entity
@Table(name = "roles")
data class Role(
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    var id: Long = 0,

    @Enumerated(EnumType.STRING)
    @Column(length = 20, nullable = false, unique = true)
    var name: ERole
) {
    override fun toString(): String {
        return "Role(name=$name)"
    }
}

enum class ERole {
    ROLE_USER,
}
