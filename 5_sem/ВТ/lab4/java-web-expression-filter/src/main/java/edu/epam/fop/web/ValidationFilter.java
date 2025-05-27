package edu.epam.fop.web;

import java.io.IOException;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import jakarta.servlet.Filter;
import jakarta.servlet.FilterChain;
import jakarta.servlet.FilterConfig;
import jakarta.servlet.ServletContext;
import jakarta.servlet.ServletException;
import jakarta.servlet.ServletRequest;
import jakarta.servlet.ServletResponse;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

public class ValidationFilter implements Filter {

	private Pattern pattern;

	@Override
	public void init(FilterConfig filterConfig) throws ServletException {
		pattern = Pattern.compile(filterConfig.getServletContext().getInitParameter("expression-regex"));
	}

	@Override
	public void doFilter(ServletRequest request, ServletResponse response, FilterChain chain)
			throws IOException, ServletException {
		String expression = request.getParameter("expression");
		Matcher matcher = pattern.matcher(expression);

		if (!matcher.matches()) {
			((HttpServletResponse)response).sendRedirect("error.jsp");
			((HttpServletRequest)request).getSession().setAttribute("error", "Invalid expression");
			return;
		}
		chain.doFilter(request, response);
	}
}