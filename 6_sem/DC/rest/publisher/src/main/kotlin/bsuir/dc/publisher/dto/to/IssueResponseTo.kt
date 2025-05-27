package bsuir.dc.publisher.dto.to

import java.time.LocalDateTime

data class IssueResponseTo(
    val id: Long = 0,
    val writerId: Long = 0,
    val title: String = "",
    val content: String = "",
    val created: LocalDateTime = LocalDateTime.now(),
    val modified: LocalDateTime = LocalDateTime.now()
)
