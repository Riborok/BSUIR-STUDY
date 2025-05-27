package com.task_management.user_service.entity

import jakarta.persistence.*

@Entity
@Table(name = "profiles")
data class Profile(
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    var id: Long = 0,

    @Column(nullable = false)
    var firstName: String,

    @Column(nullable = false)
    var lastName: String,

    @OneToOne(fetch = FetchType.LAZY, cascade = [CascadeType.ALL], orphanRemoval = true)
    @JoinColumn(name = "user_id", nullable = false)
    var user: User? = null
) {
    override fun toString(): String {
        return "Profile(firstName=$firstName, lastName=$lastName)"
    }
}
