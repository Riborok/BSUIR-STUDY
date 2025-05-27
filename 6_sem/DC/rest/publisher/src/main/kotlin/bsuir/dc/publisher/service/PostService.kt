package bsuir.dc.publisher.service

import bsuir.dc.publisher.dto.from.PostRequestTo
import bsuir.dc.publisher.dto.to.PostResponseTo
import bsuir.dc.publisher.mapper.toEntity
import bsuir.dc.publisher.mapper.toResponse
import bsuir.dc.publisher.repository.IssueRepository
import bsuir.dc.publisher.repository.PostRepository
import org.springframework.cache.annotation.CacheEvict
import org.springframework.cache.annotation.CachePut
import org.springframework.cache.annotation.Cacheable
import org.springframework.cache.annotation.Caching
import org.springframework.kafka.core.KafkaTemplate
import org.springframework.stereotype.Service

@Service
class PostService(
    private val postRepository: PostRepository,
    private val issueRepository: IssueRepository,
    private val kafkaTemplate: KafkaTemplate<String, Any>
) {
    @Caching(
        evict = [
            CacheEvict(value = ["posts"], key = "'all_posts'"),
            CacheEvict(value = ["postsByIssue"], key = "#postRequestTo.issueId")
        ]
    )
    fun createPost(postRequestTo: PostRequestTo): PostResponseTo {
        val issue = issueRepository.findById(postRequestTo.issueId).orElseThrow { NoSuchElementException() }
        val post = postRequestTo.toEntity(issue)
        val savedPost = postRepository.save(post)
        val postResponse = savedPost.toResponse()
        sendPostCreatedEvent(postResponse)
        return postResponse
    }

    @Cacheable(value = ["posts"], key = "#id")
    fun getPostById(id: Long): PostResponseTo {
        val post = postRepository.findById(id).orElseThrow { NoSuchElementException() }
        return post.toResponse()
    }

    @Cacheable(value = ["posts"], key = "'all_posts'")
    fun getAllPosts(): List<PostResponseTo> =
        postRepository.findAll().map { it.toResponse() }

    @Caching(
        put = [CachePut(value = ["posts"], key = "#id")],
        evict = [
            CacheEvict(value = ["posts"], key = "'all_posts'"),
            CacheEvict(value = ["postsByIssue"], key = "#postRequestTo.issueId")
        ]
    )
    fun updatePost(id: Long, postRequestTo: PostRequestTo): PostResponseTo {
        val issue = issueRepository.findById(postRequestTo.issueId).orElseThrow { NoSuchElementException() }
        val updatedPost = postRequestTo.toEntity(issue).apply { this.id = id }
        val postResponse = postRepository.save(updatedPost).toResponse()
        sendPostUpdatedEvent(postResponse)
        return postResponse
    }

    @Caching(
        evict = [
            CacheEvict(value = ["posts"], key = "#id"),
            CacheEvict(value = ["posts"], key = "'all_posts'"),
            CacheEvict(value = ["postsByIssue"], allEntries = true)
        ]
    )
    fun deletePost(id: Long) {
        postRepository.findById(id).orElseThrow { NoSuchElementException() }
        postRepository.deleteById(id)
        sendPostDeletedEvent(id)
    }

    @Cacheable(value = ["postsByIssue"], key = "#issueId")
    fun getPostsByIssueId(issueId: Long): List<PostResponseTo> {
        val issue = issueRepository.findById(issueId).orElseThrow { NoSuchElementException() }
        return issue.posts.map { it.toResponse() }
    }

    private fun sendPostCreatedEvent(post: PostResponseTo) {
        kafkaTemplate.send("post-create", post)
    }

    private fun sendPostUpdatedEvent(post: PostResponseTo) {
        kafkaTemplate.send("post-update", post)
    }

    private fun sendPostDeletedEvent(postId: Long) {
        kafkaTemplate.send("post-delete", postId)
    }
}