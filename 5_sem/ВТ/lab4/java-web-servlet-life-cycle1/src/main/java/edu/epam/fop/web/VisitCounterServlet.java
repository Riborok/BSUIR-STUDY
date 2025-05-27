package edu.epam.fop.web;

import java.io.IOException;
import java.io.PrintWriter;
import java.util.concurrent.atomic.AtomicInteger;

import jakarta.servlet.ServletContext;
import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

@WebServlet("/VisitCounter")
public class VisitCounterServlet extends HttpServlet {
	private static final long serialVersionUID = 7485830631456155452L;

	private static final String VISIT_COUNTER_ATTRIBUTE_NAME = "visitCount";
	private static final String COUNTER_PATH = "/WEB-INF/visitCount.txt";

	// Use this object for logging.
	System.Logger logger = System.getLogger(VisitCounterServlet.class.getName());

	// Use this object to save and restore a visit counter.
	CounterFileHelper fileHelper = new CounterFileHelper();

	// Use this object to place a visit counter in a servlet context as an
	// attribute.
	AtomicInteger visitCount;

	private String counterRealPath;

	@Override
	public void init() throws ServletException {
		ServletContext context = getServletContext();
		counterRealPath = context.getRealPath(COUNTER_PATH);

		try {visitCount = ((AtomicInteger)context.getAttribute(VISIT_COUNTER_ATTRIBUTE_NAME));
			if (visitCount == null) {
				throw new ServletException("No visitCount attribute found");
			}
		} catch (Exception ex) {
			visitCount = new AtomicInteger(0);
			visitCount.set(fileHelper.restoreCount(counterRealPath));
			context.setAttribute(VISIT_COUNTER_ATTRIBUTE_NAME, visitCount);
		}
		String msg = "Inited with " + visitCount.get() + " visits.";
		logger.log(System.Logger.Level.DEBUG, msg);
	}
	@Override
	protected void service(HttpServletRequest request, HttpServletResponse response)
			throws ServletException, IOException {


		int currentCounter = visitCount.incrementAndGet();

		response.setContentType("text/html");
		PrintWriter out = response.getWriter();
		out.println("<!DOCTYPE html><html lang=\"en\"><head><title>Visit Counter</title></head><body>");
		out.println("<h2>Visit counter: " + currentCounter + "</h2>");
		out.println("</body></html>");

		String msg = "Handling request from user";
		logger.log(System.Logger.Level.DEBUG, msg);

	}

	@Override
	public void destroy() {
		try {
			fileHelper.saveCount(counterRealPath, visitCount.get());
			String msg = "Servlet was destroyed successfully";
			logger.log(System.Logger.Level.DEBUG, msg);
		} catch (ServletException e) {
			throw new RuntimeException(e);
		}
	}
}