package com.task_management.task_service.entity

import jakarta.persistence.*
import java.time.LocalDateTime

@Entity
@Table(name = "tasks")
data class Task(
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    var id: Long = 0,

    @Column(nullable = false)
    var title: String,

    @Column(nullable = false, length = 4096)
    var description: String,

    @Column(nullable = false)
    var status: String,

    @Column(nullable = false)
    var createdBy: String,

    @Column(nullable = false)
    var createdAt: LocalDateTime = LocalDateTime.now(),

    @ElementCollection(fetch = FetchType.EAGER)
    @CollectionTable(name = "task_participants", joinColumns = [JoinColumn(name = "task_id")])
    @Column(name = "user_id", nullable = false)
    var participants: MutableSet<String> = mutableSetOf(),

    @OneToMany(mappedBy = "task", fetch = FetchType.LAZY, cascade = [CascadeType.ALL], orphanRemoval = true)
    var history: MutableList<TaskHistory> = mutableListOf()
)
