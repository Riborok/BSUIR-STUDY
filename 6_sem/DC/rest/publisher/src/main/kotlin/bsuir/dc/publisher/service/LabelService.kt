package bsuir.dc.publisher.service

import bsuir.dc.publisher.dto.from.LabelRequestTo
import bsuir.dc.publisher.dto.to.LabelResponseTo
import bsuir.dc.publisher.mapper.toEntity
import bsuir.dc.publisher.mapper.toResponse
import bsuir.dc.publisher.repository.IssueRepository
import bsuir.dc.publisher.repository.LabelRepository
import org.springframework.cache.annotation.CacheEvict
import org.springframework.cache.annotation.CachePut
import org.springframework.cache.annotation.Cacheable
import org.springframework.cache.annotation.Caching
import org.springframework.stereotype.Service

@Service
class LabelService(
    private val labelRepository: LabelRepository,
    private val issueRepository: IssueRepository,
) {
    @Caching(
        evict = [
            CacheEvict(value = ["labels"], key = "'all_labels'")
        ]
    )
    fun createLabel(labelRequestTo: LabelRequestTo): LabelResponseTo {
        val label = labelRequestTo.toEntity()
        val savedLabel = labelRepository.save(label)
        return savedLabel.toResponse()
    }

    @Cacheable(value = ["labels"], key = "#id")
    fun getLabelById(id: Long): LabelResponseTo {
        val label = labelRepository.findById(id).orElseThrow { NoSuchElementException() }
        return label.toResponse()
    }

    @Cacheable(value = ["labels"], key = "'all_labels'")
    fun getAllLabels(): List<LabelResponseTo> =
        labelRepository.findAll().map { it.toResponse() }

    @Caching(
        put = [CachePut(value = ["labels"], key = "#id")],
        evict = [CacheEvict(value = ["labels"], key = "'all_labels'")]
    )
    fun updateLabel(id: Long, labelRequestTo: LabelRequestTo): LabelResponseTo {
        val updatedLabel = labelRequestTo.toEntity().apply { this.id = id }
        return labelRepository.save(updatedLabel).toResponse()
    }

    @Caching(
        evict = [
            CacheEvict(value = ["labels"], key = "#id"),
            CacheEvict(value = ["labels"], key = "'all_labels'")
        ]
    )
    fun deleteLabel(id: Long) {
        labelRepository.findById(id).orElseThrow { NoSuchElementException() }
        labelRepository.deleteById(id)
    }

    @Cacheable(value = ["labelsByIssue"], key = "#issueId")
    fun getLabelsByIssueId(issueId: Long): List<LabelResponseTo> {
        val issue = issueRepository.findById(issueId).orElseThrow { NoSuchElementException() }
        return issue.labels.map { it.toResponse() }
    }
}