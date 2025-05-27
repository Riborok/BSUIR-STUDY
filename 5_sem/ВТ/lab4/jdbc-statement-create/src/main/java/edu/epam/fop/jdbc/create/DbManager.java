package edu.epam.fop.jdbc.create;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.List;

public class DbManager {

	private DbManager() {
	}

	public static List<Group> fetchGroups(Connection connection) throws SQLException {
		List<Group> groupList = new ArrayList<>();
		String sql = "SELECT id, group_name FROM groups";

		try (Statement stmt = connection.createStatement();
			 ResultSet rs = stmt.executeQuery(sql)) {

			while (rs.next()) {
				int id = rs.getInt("id");
				String name = rs.getString("group_name");

				Group grp = new Group(id, name);
				groupList.add(grp);
			}
		}

		return groupList;
	}

	public static List<Student> fetchStudents(Connection connection) throws SQLException {
		List<Student> studentList = new ArrayList<>();
		String sql = "SELECT id, first_name, last_name, group_id FROM students";

		try (Statement stmt = connection.createStatement();
			 ResultSet rs = stmt.executeQuery(sql)) {

			while (rs.next()) {
				int id = rs.getInt("id");
				int grpId = rs.getInt("group_id");
				String firstName = rs.getString("first_name");
				String lastName = rs.getString("last_name");

				String groupSql = "SELECT g.group_name FROM groups g WHERE g.id = " + grpId;
				try (Statement grpStmt = connection.createStatement();
					 ResultSet grpRs = grpStmt.executeQuery(groupSql)) {

					String grpName = "";
					if (grpRs.next()) {
						grpName = grpRs.getString("group_name");
					}
					Student stdnt = new Student(id, firstName, lastName, new Group(grpId, grpName));
					studentList.add(stdnt);
				}
			}
		}
		return studentList;
	}
}