package edu.epam.fop.web;

import java.io.IOException;
import java.util.concurrent.ThreadLocalRandom;

import jakarta.servlet.ServletConfig;
import jakarta.servlet.ServletContext;
import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

@WebServlet("/GuessNumber")
public class GuessNumberController extends HttpServlet {
	private static final long serialVersionUID = -1963426021633719325L;

	private static final int MAX_NUMBER_OF_ATTEMPTS = 10;

	private static final String HOME_PAGE = "/index.html";
	private static final String GAME_START_PAGE = "/WEB-INF/start.jsp";
	private static final String GAME_ATTEMPT_PAGE = "/WEB-INF/tryIt.jsp";
	private static final String GAME_END_PAGE = "/WEB-INF/finish.jsp";

	private static final String START_COMMAND = "start";
	private static final String TRY_IT_COMMAND = "tryIt";

	private static final String MIN_NUMBER_PARAMETER = "minNumber";
	private static final String MAX_NUMBER_PARAMETER = "maxNumber";
	private static final String COMMAND_PARAMETER = "command";
	private static final String NUMBER_PARAMETER = "number";
	private static final String USER_NAME_PARAMETER = "name";

	private static final String MIN_NUMBER_ATTRIBUTE = "minNumber";
	private static final String MAX_NUMBER_ATTRIBUTE = "maxNumber";
	private static final String USER_NAME_ATTRIBUTE = "name";
	private static final String RESULT_ATTRIBUTE = "result";
	private static final String TRY_COUNT_ATTRIBUTE = "tryCount";
	private static final String MAX_NUMBER_OF_ATTEMPTS_ATTRIBUTE = "maxAttempts";

	private static final String MISSING_MESSAGE = "Please enter a number";
	private static final String LESS_MESSAGE = "Your number is less than the number being guessed";
	private static final String WIN_MESSAGE = "You win!";
	private static final String GREATER_MESSAGE = "Your number is greater than the number being guessed";
	private static final String INVALID_MESSAGE = "Invalid value entered";
	private static final String LOSE_MESSAGE = "You lose";

	enum Status {
		MISSING, LESS, WIN, GREATER, INVALID, LOSE
	}

	// Use this object for logging.
	System.Logger logger = System.getLogger(GuessNumberController.class.getName());

	// Use this field to store user name.
	String name;

	// Use this field to store the lower limit of a random number range.
	int minNumber;

	// Use this field to store the upper limit of a random number range.
	int maxNumber;

	// Use this field to store a random (secret) number.
	int randomNumber;

	// Use this field as the counter of attempts.
	int tryCount;

	@Override
	public void init() throws ServletException {
		ServletContext context = getServletContext();
		context.setAttribute(MAX_NUMBER_OF_ATTEMPTS_ATTRIBUTE, MAX_NUMBER_OF_ATTEMPTS);
		ServletConfig config = getServletConfig();
		String temp = config.getInitParameter(MIN_NUMBER_PARAMETER);
		if (temp == null) {
			minNumber = 1;
		} else {
			minNumber = Integer.parseInt(temp);
		}
		temp = config.getInitParameter(MAX_NUMBER_PARAMETER);
		if(temp == null) {
			maxNumber = 50;
		} else {
			maxNumber = Integer.parseInt(temp);
		}

		context.setAttribute(MIN_NUMBER_ATTRIBUTE, minNumber);
		context.setAttribute(MAX_NUMBER_ATTRIBUTE, maxNumber);

		logger.log(System.Logger.Level.DEBUG, "Initialization complete with minNumber = " + minNumber + ", maxNumber = " + maxNumber);
	}

	@Override
	public void destroy() {
		String msg = "Servlet destroyed";
		logger.log(System.Logger.Level.DEBUG, msg);
	}

	@Override
	protected void service(HttpServletRequest request, HttpServletResponse response)
			throws ServletException, IOException {

		String command = request.getParameter(COMMAND_PARAMETER);
		if (command == null) {
			command = "";
		}
		String pathToForward = switch (command) {
            case START_COMMAND -> handleStart(request);
            case TRY_IT_COMMAND -> handleTry(request);
            default -> handleDefault();
        };
        request.getRequestDispatcher(pathToForward).forward(request, response);
		logger.log(System.Logger.Level.INFO,
				"minNumber: {0}, maxNumber: {1}, randomNumber: {2}, name: {3}, tryCount: {4}, command: {5}, page: {6}",
				minNumber, maxNumber, randomNumber, name, tryCount, command, pathToForward);
	}

	String handleDefault() {
		return HOME_PAGE;
	}

	String handleStart(HttpServletRequest request) {
		tryCount = 0;
		name = request.getParameter(USER_NAME_PARAMETER);

		request.setAttribute(USER_NAME_ATTRIBUTE, name);
		randomNumber = ThreadLocalRandom.current().nextInt(minNumber, maxNumber);

		return GAME_START_PAGE;
	}

	String handleTry(HttpServletRequest request) {
		String page = null;
		String enteredString = request.getParameter(NUMBER_PARAMETER);
		Status status = handleEnteredNumber(enteredString);

		switch (status) {
			case MISSING:
				page = GAME_ATTEMPT_PAGE;
				request.setAttribute(RESULT_ATTRIBUTE, MISSING_MESSAGE);
				break;
			case INVALID:
				page = GAME_ATTEMPT_PAGE;
				request.setAttribute(RESULT_ATTRIBUTE, INVALID_MESSAGE);
				break;
			case LESS:
				page = GAME_ATTEMPT_PAGE;
				request.setAttribute(RESULT_ATTRIBUTE, LESS_MESSAGE);
				break;
			case GREATER:
				page = GAME_ATTEMPT_PAGE;
				request.setAttribute(RESULT_ATTRIBUTE, GREATER_MESSAGE);
				break;
			case WIN:
				page = GAME_END_PAGE;
				request.setAttribute(RESULT_ATTRIBUTE, WIN_MESSAGE);
				break;
			case LOSE:
				page = GAME_END_PAGE;
				request.setAttribute(RESULT_ATTRIBUTE, LOSE_MESSAGE);
				break;
			default:
				page = HOME_PAGE;
				break;
		}

		request.setAttribute(TRY_COUNT_ATTRIBUTE, ++tryCount);
		String msg = "Handle try with status = " + status;
		logger.log(System.Logger.Level.DEBUG, msg);
		return page;
	}

	private Status handleEnteredNumber(String enteredString) {
		Status status;
		if (enteredString == null) {
			status = Status.MISSING;
		} else {
			try {
				int enteredNumber = Integer.parseInt(enteredString);
				status = (enteredNumber < randomNumber) ? Status.LESS
						: ((enteredNumber == randomNumber) ? Status.WIN : Status.GREATER);
			} catch (NumberFormatException e) {
				status = Status.INVALID;
			}
		}
		if ((tryCount > MAX_NUMBER_OF_ATTEMPTS) || (tryCount == MAX_NUMBER_OF_ATTEMPTS && status != Status.WIN)) {
			status = Status.LOSE;
		}
		return status;
	}
}