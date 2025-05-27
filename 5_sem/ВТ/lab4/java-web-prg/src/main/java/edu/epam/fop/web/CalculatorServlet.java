package edu.epam.fop.web;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import jakarta.servlet.http.HttpSession;

public class CalculatorServlet extends HttpServlet {
	protected void doGet(HttpServletRequest request, HttpServletResponse response)
			throws IOException {
		HttpSession session = request.getSession();
		session.setAttribute("error", "Please use the POST method for calculations");
		response.sendRedirect("result.jsp");
	}

	protected void doPost(HttpServletRequest request, HttpServletResponse response)
			throws IOException {
		HttpSession session = request.getSession();
		if (request.getParameter("action") != null && request.getParameter("action").equals("clearHistory")){
			session.removeAttribute("history");
			response.sendRedirect("result.jsp");
			return;
		}
		String expression = request.getParameter("expression");
		String[] res = expression.split("[-/+*]");

		char action = ' ';
		Pattern pattern = Pattern.compile("([-+*/])");
		Matcher matcher = pattern.matcher(expression);
		if (matcher.find()) {
			action = matcher.group(1).charAt(0);
		}

		double num1 = 1, num2 = 1;
		boolean flag = false;
		try {
			num1 = Double.parseDouble(res[0]);
			num2 = Double.parseDouble(res[1]);
		} catch (Exception e) {
			flag = true;
		}
		if (action == ' ' || flag){
			session.setAttribute("error", "Invalid expression: " + expression);
			response.sendRedirect("result.jsp");
			return;
		}
		double expressionResult = 0;
		try {
			switch (action) {
				case '-': {
					expressionResult = num1 - num2;
					break;
				}
				case '+': {
					expressionResult = num1 + num2;
					break;
				}
				case '/': {
					if(num2 == 0){
						throw new ArithmeticException("Division by zero");
					}
					expressionResult = num1 / num2;
					break;
				}
				case '*': {
					expressionResult = num1 * num2;
					break;
				}
			}
		} catch (Exception e) {
			session.setAttribute("error", e.getMessage());
			response.sendRedirect("result.jsp");
			return;
		}
		session.removeAttribute("error");
		List<String> historyList = (List<String>)session.getAttribute("history");
		if (historyList == null){
			historyList = new ArrayList<String>();
		}
		historyList.add(expression + "=" + expressionResult);
		session.setAttribute("history", historyList);
		response.sendRedirect("result.jsp");
	}
}