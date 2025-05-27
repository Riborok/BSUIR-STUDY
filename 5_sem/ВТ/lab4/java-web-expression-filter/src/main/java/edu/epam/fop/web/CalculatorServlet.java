package edu.epam.fop.web;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import jakarta.servlet.ServletContext;
import jakarta.servlet.ServletException;
import jakarta.servlet.SessionCookieConfig;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import jakarta.servlet.http.HttpSession;

public class CalculatorServlet extends HttpServlet {
	private static final long serialVersionUID = 1L;

	private Pattern pattern;

	@Override
	public void init() throws ServletException {
		ServletContext context = getServletContext();
		String regex = context.getInitParameter("expression-regex");
		pattern = Pattern.compile(regex);
	}

	protected void doGet(HttpServletRequest request, HttpServletResponse response)
			throws ServletException, IOException {
		request.getSession().setAttribute("error", "Please use the POST method for calculations");
		response.sendRedirect("error.jsp");
	}

	protected void doPost(HttpServletRequest request, HttpServletResponse response)
			throws ServletException, IOException {
		String action = request.getParameter("action");
		if(action != null && action.equals("clearHistory")){
			request.getSession().removeAttribute("history");
			response.sendRedirect("result.jsp");
			return;
		}
		String expression = request.getParameter("expression");
		Matcher matcher = pattern.matcher(expression);
		double result = 0;
		try {
			if (!matcher.matches()) {
				throw new IllegalArgumentException("Invalid expression: \"" + expression + "\"");
			}
			String operator = matcher.group("operation");
			double operand1 = Double.parseDouble(matcher.group("operand1"));
			double operand2 = Double.parseDouble(matcher.group("operand2"));
			switch (operator) {
				case "+":{
					result = operand1 + operand2;
					break;
				}
				case "-":{
					result = operand1 - operand2;
					break;
				}
				case "*":{
					result = operand1 * operand2;
					break;
				}
				case "/":{
					if (operand2 == 0) {
						throw new IllegalArgumentException("Invalid expression: \"" + expression + "\"");
					}
					result = operand1 / operand2;
					break;
				}
			}
		} catch (Exception e) {
			request.getSession().setAttribute("error", e.getMessage());
			response.sendRedirect("error.jsp");
			return;
		}

		HttpSession session= request.getSession();
		List<String> history = (List<String>)session.getAttribute("history");
		if (history == null){
			history = new ArrayList<>();
		}
		history.add(expression + '=' + result);
		session.setAttribute("history", history);
		session.removeAttribute("error");
		response.sendRedirect("result.jsp");
	}
}