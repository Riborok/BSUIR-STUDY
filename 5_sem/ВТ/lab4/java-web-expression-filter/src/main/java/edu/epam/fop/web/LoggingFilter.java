package edu.epam.fop.web;

import java.io.IOException;
import java.util.Arrays;
import java.util.Enumeration;
import java.util.logging.Logger;

import jakarta.servlet.Filter;
import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.ServletRequest;
import jakarta.servlet.ServletResponse;

public class LoggingFilter implements Filter {
	private static final Logger LOG = Logger.getLogger(LoggingFilter.class.getName());

	protected void logRequestParameter(String paramName, String paramValues) {
		LOG.info("Request parameter " + paramName + ": " + paramValues);
	}

	@Override
	public void doFilter(ServletRequest request, ServletResponse response, FilterChain chain)
			throws IOException, ServletException {
		Enumeration<String> params = request.getParameterNames();
		while (params.hasMoreElements()) {
			String paramName = params.nextElement();
			String[] paramValue = request.getParameterValues(paramName);
			logRequestParameter(paramName, paramValue[0]);
		}
		chain.doFilter(request, response);
	}
}