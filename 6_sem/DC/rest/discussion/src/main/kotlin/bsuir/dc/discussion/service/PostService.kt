package bsuir.dc.discussion.service

import bsuir.dc.discussion.dto.from.PostRequestTo
import bsuir.dc.discussion.dto.to.PostResponseTo
import bsuir.dc.discussion.entity.PostKey
import bsuir.dc.discussion.mapper.toEntity
import bsuir.dc.discussion.mapper.toResponse
import bsuir.dc.discussion.repository.PostRepository
import org.springframework.kafka.annotation.KafkaListener
import org.springframework.stereotype.Service
import java.util.NoSuchElementException

@Service
class PostService(private val postRepository: PostRepository) {

    fun createPost(postRequestTo: PostRequestTo, country: String): PostResponseTo {
        return postRepository.save(postRequestTo.toEntity(country)).toResponse()
    }

    fun getAllPosts(): List<PostResponseTo> {
        return postRepository.findAll().map { it.toResponse() }
    }

    fun getPostById(country: String, id: Long): PostResponseTo {
        val postKey = PostKey(country, id)
        return postRepository.findById(postKey).orElseThrow {
            NoSuchElementException()
        }.toResponse()
    }

    fun deletePost(country: String, id: Long) {
        val postKey = PostKey(country, id)
        postRepository.findById(postKey).orElseThrow {
            NoSuchElementException()
        }
        postRepository.deleteById(postKey)
    }

    fun updatePost(country: String, postRequestTo: PostRequestTo): PostResponseTo {
        val postKey = PostKey(country, postRequestTo.id)
        val existingPost = postRepository.findById(postKey).orElseThrow {
            NoSuchElementException()
        }
        val updatedPost = existingPost.copy(
            issueId = postRequestTo.issueId,
            content = postRequestTo.content
        )
        return postRepository.save(updatedPost).toResponse()
    }

    @KafkaListener(topics = ["post-create"], groupId = "discussion-group")
    fun listenToPostCreate(postRequestTo: PostRequestTo) {
        createPost(postRequestTo, "Belarus")
    }

    @KafkaListener(topics = ["post-update"], groupId = "discussion-group")
    fun listenToPostUpdate(postRequestTo: PostRequestTo) {
        updatePost("Belarus", postRequestTo)
    }

    @KafkaListener(topics = ["post-delete"], groupId = "discussion-group")
    fun listenToPostDelete(id: Long) {
        deletePost("Belarus", id)
    }
}