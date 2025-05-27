package edu.epam.fop.web;

import java.io.IOException;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import edu.epam.fop.web.exceptions.InvalidISBNException;
import jakarta.servlet.RequestDispatcher;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

public class BookSearchServlet extends HttpServlet {
	private static final long serialVersionUID = 4988158674830109078L;

	private static final String ISBN_PARAMETER = "isbn";
	private static final String RESULT_ATTRIBUTE = "result";
	private static final String RESULT_PAGE = "result.jsp";

	public void doGet(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
		String isbn = request.getParameter(ISBN_PARAMETER);
		String regexp = "[0-9]{13}";
		Pattern pattern = Pattern.compile(regexp);
		Matcher matcher = pattern.matcher(isbn);
		if (matcher.matches()) {
			request.setAttribute(RESULT_ATTRIBUTE, "Book search result. ISBN: " + isbn);
			RequestDispatcher rd = request.getRequestDispatcher(RESULT_PAGE);
			rd.forward(request, response);
		} else {
			throw new InvalidISBNException("The ISBN entered is not valid. Please enter exactly 13 digits.");
		}
	}
}