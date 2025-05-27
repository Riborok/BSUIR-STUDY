package edu.epam.fop.web;

import java.io.IOException;
import java.util.Arrays;
import java.util.Enumeration;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;

import jakarta.servlet.Filter;
import jakarta.servlet.FilterChain;
import jakarta.servlet.FilterConfig;
import jakarta.servlet.ServletException;
import jakarta.servlet.ServletRequest;
import jakarta.servlet.ServletResponse;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

public class AuthorizationFilter implements Filter {
	private static final String COMMAND_PARAMETER_NAME = "command";
	private static final String USER_ROLE_ATTRIBUTE_NAME = "role";
	private static final String LOGIN_COMMAND = "login";
	private static final String LOGOUT_COMMAND = "logout";

	Map<String, Set<String>> roleCommands;

	@Override
	public void init(FilterConfig config) throws ServletException {
		roleCommands = new HashMap<>();
		Enumeration<String> paramNames = config.getInitParameterNames();

		while(paramNames.hasMoreElements()){
			String paramName = paramNames.nextElement();
			String paramValue = config.getInitParameter(paramName);
			roleCommands.put(paramName, new HashSet<>(Arrays.asList(paramValue.split(" "))));
		}
	}

	@Override
	public void doFilter(ServletRequest request, ServletResponse response, FilterChain chain)
			throws IOException, ServletException {
		HttpServletRequest httpRequest = (HttpServletRequest) request;
		String command = httpRequest.getParameter(COMMAND_PARAMETER_NAME);
		String role = (String) httpRequest.getSession().getAttribute(USER_ROLE_ATTRIBUTE_NAME);
		Set<String> allowedCommands = roleCommands.get(role);

		boolean isCommandInvalid = command == null ||
				(role == null && LOGOUT_COMMAND.equals(command)) ||
				(role != null && LOGIN_COMMAND.equals(command)) ||
				(allowedCommands == null || !allowedCommands.contains(command)) &&
						!LOGOUT_COMMAND.equals(command) && !LOGIN_COMMAND.equals(command);
		if (isCommandInvalid) {
			((HttpServletResponse) response).sendError(HttpServletResponse.SC_FORBIDDEN);
			return;
		}
		chain.doFilter(request, response);
	}
}