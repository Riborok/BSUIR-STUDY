package com.task_management.task_service.entity

import jakarta.persistence.*
import java.time.LocalDateTime

@Entity
@Table(name = "task_history")
data class TaskHistory(
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    var id: Long = 0,

    @Enumerated(EnumType.STRING)
    @Column(length = 20, nullable = false)
    var action: ActionType,

    @Column(nullable = false, length = 4096)
    var previousValue: String,

    @Column(nullable = false, length = 4096)
    var newValue: String,

    @Column(nullable = false)
    var changedBy: String,

    @Column(nullable = false)
    var changedAt: LocalDateTime = LocalDateTime.now(),

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "task_id", nullable = false)
    var task: Task
)

enum class ActionType {
    STATUS_CHANGE,
    DESCRIPTION_CHANGE,
    TITLE_CHANGE
}
