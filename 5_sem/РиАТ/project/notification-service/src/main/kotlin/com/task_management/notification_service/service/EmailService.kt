package com.task_management.notification_service.service

import jakarta.mail.internet.InternetAddress
import org.springframework.beans.factory.annotation.Value
import org.springframework.mail.MailException
import org.springframework.mail.javamail.JavaMailSender
import org.springframework.mail.javamail.MimeMessageHelper
import org.springframework.stereotype.Service
import org.slf4j.LoggerFactory

@Service
class EmailService(
    private val mailSender: JavaMailSender,
    @Value("\${spring.mail.sender.email}") private val senderEmail: String,
    @Value("\${spring.mail.sender.text}") private val senderText: String
) {
    private val logger = LoggerFactory.getLogger(EmailService::class.java)

    fun sendEmail(to: String, subject: String, body: String) {
        val message = mailSender.createMimeMessage()
        val helper = MimeMessageHelper(message, true)

        try {
            helper.setFrom(InternetAddress(senderEmail, senderText))
            helper.setTo(to)
            helper.setSubject(subject)
            helper.setText(body, true)

            logger.info("Sending email to: $to with subject: $subject")
            mailSender.send(message)
            logger.info("Email successfully sent to: $to")
        } catch (e: MailException) {
            logger.error("Failed to send email to: $to with subject: $subject", e)
        }
    }
}