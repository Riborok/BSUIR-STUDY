package edu.epam.fop.web;

import java.io.IOException;
import java.util.List;

import javax.sql.DataSource;

import jakarta.servlet.RequestDispatcher;
import jakarta.servlet.ServletContext;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

public class MyServlet extends HttpServlet {
	private static final long serialVersionUID = 3779599415327871476L;

	private static final String DATA_SOURCE_ATTRIBUTE = "dataSource";
	private static final String STUDENT_LIST_ATTRIBUTE = "studentList";
	private static final String TEACHER_LIST_ATTRIBUTE = "teacherList";
	private static final String RESULT_PAGE = "/WEB-INF/result.jsp";

	protected DbManager createDbManager(DataSource dataSource) {
		return new DbManager(dataSource);
	}

	protected void doGet(HttpServletRequest request, HttpServletResponse response)
			throws ServletException, IOException {

		DataSource dataSource = null; // Don't forget to fix it.

		ServletContext servletContext = getServletContext();
		dataSource = (DataSource) servletContext.getAttribute(DATA_SOURCE_ATTRIBUTE);

		DbManager dbManager = createDbManager(dataSource);

		// Get students.
		List<String> students = dbManager.getUsersByRole(1);

		// Get teachers.
		List<String> teachers = dbManager.getUsersByRole(2);

		request.setAttribute(STUDENT_LIST_ATTRIBUTE, students);
		request.setAttribute(TEACHER_LIST_ATTRIBUTE, teachers);
		RequestDispatcher dispatcher = request.getRequestDispatcher(RESULT_PAGE);
		dispatcher.forward(request, response);
	}
}