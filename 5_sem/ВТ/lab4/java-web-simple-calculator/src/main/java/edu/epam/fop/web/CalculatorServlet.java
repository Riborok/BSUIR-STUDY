package edu.epam.fop.web;

import java.io.IOException;

import jakarta.servlet.RequestDispatcher;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

public class CalculatorServlet extends HttpServlet {
	private static final long serialVersionUID = 1L;

	protected void doGet(HttpServletRequest request, HttpServletResponse response)
			throws ServletException, IOException {
		String num1 = request.getParameter("num1");
		String num2 = request.getParameter("num2");
		String operation = request.getParameter("operation");
		Calculation calculation = new Calculation();

		try {
			calculation.setNum1(Double.parseDouble(num1));
			calculation.setNum2(Double.parseDouble(num2));
			calculation.setOperation(operation);
		} catch (NumberFormatException e) {
			response.sendError(HttpServletResponse.SC_BAD_REQUEST, "Invalid number");
			return;
		}

		switch (operation) {
			case "add": {
				calculation.setResult(calculation.getNum1() + calculation.getNum2());
				break;
			}
			case "subtract": {
				calculation.setResult(calculation.getNum1() - calculation.getNum2());
				break;
			}
			case "multiply": {
				calculation.setResult(calculation.getNum1() * calculation.getNum2());
				break;
			}
			case "divide": {
				calculation.setResult(calculation.getNum1() / calculation.getNum2());
				break;
			}
			case "unknown": {
				response.sendError(HttpServletResponse.SC_BAD_REQUEST, "Unknown operation" );
				return;
			}
		}
		request.setAttribute("calculation", calculation);

		RequestDispatcher view = request.getRequestDispatcher("result.jsp");
		view.forward(request, response);
	}

	protected void doPost(HttpServletRequest request, HttpServletResponse response)
			throws ServletException, IOException {
		doGet(request, response);
	}
}