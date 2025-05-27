package bsuir.dc.publisher

import org.springframework.boot.autoconfigure.SpringBootApplication
import org.springframework.boot.runApplication
import org.springframework.cache.annotation.EnableCaching

@SpringBootApplication
@EnableCaching
class PublisherApplication

fun main(args: Array<String>) {
	runApplication<PublisherApplication>(*args)
}
