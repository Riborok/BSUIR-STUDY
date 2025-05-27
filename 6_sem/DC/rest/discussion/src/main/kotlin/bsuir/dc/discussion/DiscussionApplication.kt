package bsuir.dc.discussion

import org.springframework.boot.autoconfigure.SpringBootApplication
import org.springframework.boot.runApplication
import org.springframework.retry.annotation.EnableRetry

@SpringBootApplication
@EnableRetry
class DiscussionApplication

fun main(args: Array<String>) {
    runApplication<DiscussionApplication>(*args)
}
