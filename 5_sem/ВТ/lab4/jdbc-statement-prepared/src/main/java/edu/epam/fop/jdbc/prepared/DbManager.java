package edu.epam.fop.jdbc.prepared;

import java.sql.*;
import java.util.ArrayList;
import java.util.List;

public class DbManager {

	private DbManager() {
	}

	public static boolean insertGroup(Connection connection, Group group) throws SQLException {
		int result = 0;
		String sql = "INSERT INTO groups (group_name) VALUES (?)";
		try (PreparedStatement pstmt = connection.prepareStatement(sql, Statement.RETURN_GENERATED_KEYS)) {
			pstmt.setString(1, group.getName());
			result = pstmt.executeUpdate();
			try (ResultSet rs = pstmt.getGeneratedKeys()) {
				if (rs.next()) {
					group.setId(rs.getInt(1));
				}
			}
		}
		return result > 0;
	}

	public static boolean insertStudent(Connection connection, Student student) throws SQLException {
		int result = 0;
		String sql = "INSERT INTO students (first_name, last_name, group_id) VALUES (?, ?, ?)";
		try (PreparedStatement pstmt = connection.prepareStatement(sql, Statement.RETURN_GENERATED_KEYS)) {
			pstmt.setString(1, student.getFirstName());
			pstmt.setString(2, student.getLastName());
			pstmt.setInt(3, student.getGroup().getId());
			result = pstmt.executeUpdate();
			try (ResultSet rs = pstmt.getGeneratedKeys()) {
				if (rs.next()) {
					student.setId(rs.getInt(1));
				}
			}
		}
		return result > 0;
	}

	public static Group findFirstGroupByName(Connection connection, String name) throws SQLException {
		Group grp = null;
		String sql = "SELECT TOP 1 * FROM groups WHERE group_name = ?";
		try (PreparedStatement pstmt = connection.prepareStatement(sql)) {
			pstmt.setString(1, name);
			try (ResultSet rs = pstmt.executeQuery();) {
				if (rs.next()) {
					grp = new Group(rs.getInt("id"), rs.getString("group_name"));
				}
			}
		}
		return grp;
	}

	public static Student findFirstStudentByName(Connection connection, String firstName, String lastName)
			throws SQLException {
		Student st = null;
		String sql = "SELECT TOP 1 * FROM students WHERE first_name = ? AND last_name = ?";
		try (PreparedStatement pstmt = connection.prepareStatement(sql)) {
			pstmt.setString(1, firstName);
			pstmt.setString(2, lastName);
			try (ResultSet rs = pstmt.executeQuery();) {
				if (rs.next()) {
					int id = rs.getInt("id");
					int groupId = rs.getInt("group_id");
					String groupName = getGroupNameById(connection, groupId);
					st = new Student(id, firstName, lastName, new Group(groupId, groupName));
				}
			}
		}
		return st;
	}

	public static List<Student> findStudentsByGroup(Connection connection, Group group) throws SQLException {
		List<Student> studentList = new ArrayList<>();
		String sql = "SELECT * FROM students WHERE group_id = ?";
		try (PreparedStatement pstmt = connection.prepareStatement(sql)) {
			pstmt.setInt(1, group.getId());
			try (ResultSet rs = pstmt.executeQuery();) {
				while (rs.next()) {
					int id = rs.getInt("id");
					String firstName = rs.getString("first_name");
					String lastName = rs.getString("last_name");
					int groupId = rs.getInt("group_id");
					String groupName = getGroupNameById(connection, groupId);
					studentList.add(new Student(id, firstName, lastName, new Group(groupId, groupName)));
				}
			}
		}
		return studentList;
	}

	private static String getGroupNameById(Connection connection, int groupId) throws SQLException {
		String name = "";
		String sql = "SELECT g.group_name FROM groups g WHERE g.id = ?";
		try (PreparedStatement pstmt = connection.prepareStatement(sql)) {
			pstmt.setInt(1, groupId);
			try (ResultSet rs = pstmt.executeQuery();) {
				if (rs.next()) {
					name = rs.getString("group_name");
				}
			}
		}
		return name;
	}

	public static boolean updateGroupById(Connection connection, Group group) throws SQLException {
		int result = 0;
		String sql = "UPDATE groups SET group_name = ? WHERE id = ?";
		try (PreparedStatement pstmt = connection.prepareStatement(sql)) {
			pstmt.setString(1, group.getName());
			pstmt.setInt(2, group.getId());
			result = pstmt.executeUpdate();
		}
		return result > 0;
	}

	public static boolean updateStudentById(Connection connection, Student student) throws SQLException {
		int result = 0;
		String sql = "UPDATE students SET first_name = ?, last_name = ?, group_id = ? WHERE id = ?";
		try (PreparedStatement pstmt = connection.prepareStatement(sql)) {
			pstmt.setString(1, student.getFirstName());
			pstmt.setString(2, student.getLastName());
			pstmt.setInt(3, student.getGroup().getId());
			pstmt.setInt(4, student.getId());
			result = pstmt.executeUpdate();
		}
		return result > 0;
	}
}