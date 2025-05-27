package edu.epam.fop.jdbc.transactions;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.SQLException;
import java.util.List;

public class DbManager {

	private DbManager() {
		throw new UnsupportedOperationException();
	}

	public static boolean setGroupForStudents(Connection connection, Group group, List<Student> students)
			throws SQLException {
		boolean wasAutoCommitEnabled = false;
		String query = "UPDATE students SET group_id = ? WHERE id = ?";
		try (PreparedStatement statement = connection.prepareStatement(query)) {
			if (connection.getAutoCommit()) {
				connection.setAutoCommit(false);
				wasAutoCommitEnabled = true;
			}
			boolean res = true;
			for (Student student : students) {
				statement.setInt(2, student.getId());
				statement.setInt(1, group.getId());
				if (statement.executeUpdate() == 0) {
					res = false;
					break;
				}
			}
			if (res) {
				connection.commit();
			} else {
				connection.rollback();
			}
			return res;
		} finally {
			if (wasAutoCommitEnabled)
				connection.setAutoCommit(true);
		}
	}

	public static boolean deleteStudents(Connection connection, List<Student> students) throws SQLException {
		boolean wasAutoCommitEnabled = false;
		String query = "DELETE FROM students WHERE id = ?";
		try (PreparedStatement statement = connection.prepareStatement(query)) {
			if (connection.getAutoCommit()) {
				connection.setAutoCommit(false);
				wasAutoCommitEnabled = true;
			}
			boolean res = true;
			for (Student student : students) {
				statement.setInt(1, student.getId());
				if (statement.executeUpdate() == 0) {
					res = false;
					break;
				}
			}
			if (res) {
				connection.commit();
			} else {
				connection.rollback();
			}
			return res;
		} finally {
			if (wasAutoCommitEnabled)
				connection.setAutoCommit(true);
		}
	}
}