package edu.epam.fop.http.client;

import java.net.URI;
import java.net.http.HttpRequest;

public class TaskManager {

	public static HttpRequest createTask(String baseUrl, String taskId, String taskName, String taskStatus) {
		String requestBody = String.format("{\"taskId\":\"%s\",\"taskName\":\"%s\",\"taskStatus\":\"%s\"}",
				taskId, taskName, taskStatus);
		return HttpRequest.newBuilder()
				.uri(URI.create(baseUrl))
				.header("Content-Type", "application/json")
				.POST(HttpRequest.BodyPublishers.ofString(requestBody))
				.build();
	}

	public static HttpRequest retrieveTask(String baseUrl, String taskId) {
		String resultingUrl = baseUrl + "/" + taskId;
		return HttpRequest.newBuilder()
				.uri(URI.create(resultingUrl))
				.header("Content-Type", "application/json")
				.GET()
				.build();
	}

	public static HttpRequest updateTask(String baseUrl, String taskId, String taskName, String taskStatus) {
		String resultingUrl = baseUrl + "/" + taskId;
		String requestBody = String.format("{\"taskId\":\"%s\",\"taskName\":\"%s\",\"taskStatus\":\"%s\"}",
				taskId, taskName, taskStatus);
		return HttpRequest.newBuilder()
				.uri(URI.create(resultingUrl))
				.header("Content-Type", "application/json")
				.PUT(HttpRequest.BodyPublishers.ofString(requestBody))
				.build();
	}

	public static HttpRequest deleteTask(String baseUrl, String taskId) {
		String resultingUrl = baseUrl + "/" + taskId;
		return HttpRequest.newBuilder()
				.uri(URI.create(resultingUrl))
				.header("Content-Type", "application/json")
				.DELETE()
				.build();
	}

}