package bsuir.dc.publisher.service

import bsuir.dc.publisher.dto.from.WriterRequestTo
import bsuir.dc.publisher.dto.to.WriterResponseTo
import bsuir.dc.publisher.mapper.toEntity
import bsuir.dc.publisher.mapper.toResponse
import bsuir.dc.publisher.repository.IssueRepository
import bsuir.dc.publisher.repository.WriterRepository
import org.springframework.cache.annotation.CacheEvict
import org.springframework.cache.annotation.CachePut
import org.springframework.cache.annotation.Cacheable
import org.springframework.cache.annotation.Caching
import org.springframework.stereotype.Service

@Service
class WriterService(
    private val writerRepository: WriterRepository,
    private val issueRepository: IssueRepository,
) {
    @Caching(
        evict = [
            CacheEvict(value = ["writers"], key = "'all_writers'")
        ]
    )
    fun createWriter(writerRequestTo: WriterRequestTo): WriterResponseTo {
        val writer = writerRequestTo.toEntity()
        val savedWriter = writerRepository.save(writer)
        return savedWriter.toResponse()
    }

    @Cacheable(value = ["writers"], key = "#id")
    fun getWriterById(id: Long): WriterResponseTo {
        val writer = writerRepository.findById(id).orElseThrow { NoSuchElementException() }
        return writer.toResponse()
    }

    @Cacheable(value = ["writers"], key = "'all_writers'")
    fun getAllWriters(): List<WriterResponseTo> =
        writerRepository.findAll().map { it.toResponse() }

    @Caching(
        put = [CachePut(value = ["writers"], key = "#id")],
        evict = [CacheEvict(value = ["writers"], key = "'all_writers'")]
    )
    fun updateWriter(id: Long, writerRequestTo: WriterRequestTo): WriterResponseTo {
        val updatedWriter = writerRequestTo.toEntity().apply { this.id = id }
        return writerRepository.save(updatedWriter).toResponse()
    }

    @Caching(
        evict = [
            CacheEvict(value = ["writers"], key = "#id"),
            CacheEvict(value = ["writers"], key = "'all_writers'")
        ]
    )
    fun deleteWriter(id: Long) {
        writerRepository.findById(id).orElseThrow { NoSuchElementException() }
        writerRepository.deleteById(id)
    }

    @Cacheable(value = ["writersByIssue"], key = "#issueId")
    fun getWriterByIssueId(issueId: Long): WriterResponseTo? {
        val issue = issueRepository.findById(issueId).orElseThrow { NoSuchElementException() }
        return issue.writer.toResponse()
    }
}