package edu.epam.fop.web;

import java.util.concurrent.atomic.AtomicInteger;

import jakarta.servlet.ServletContext;
import jakarta.servlet.http.HttpSessionEvent;
import jakarta.servlet.http.HttpSessionListener;

public class MyHttpSessionListener implements HttpSessionListener {

	private static final String ACTIVE_USERS_COUNTER_ATTRIBUTE = "activeUsersCounter";

	@Override
	public void sessionCreated(HttpSessionEvent sessionEvent) {
		ServletContext context = sessionEvent.getSession().getServletContext();
		AtomicInteger activeUsers = (AtomicInteger)context.getAttribute(ACTIVE_USERS_COUNTER_ATTRIBUTE);
		activeUsers.incrementAndGet();
	}

	@Override
	public void sessionDestroyed(HttpSessionEvent sessionEvent) {
		ServletContext context = sessionEvent.getSession().getServletContext();
		AtomicInteger activeUsers = (AtomicInteger)context.getAttribute(ACTIVE_USERS_COUNTER_ATTRIBUTE);
		activeUsers.decrementAndGet();
	}
}