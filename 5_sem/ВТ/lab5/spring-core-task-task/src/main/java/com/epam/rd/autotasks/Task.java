package com.epam.rd.autotasks;

public class Task {
    private String description;
    private Employee assignee;
    private Employee reviewer;

    public Task() {
    }

    public Task(String description, Employee assignee, Employee reviewer) {
        this.description = description;
        this.assignee = assignee;
        this.reviewer = reviewer;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public Employee getAssignee() {
        return assignee;
    }

    public void setAssignee(Employee assignee) {
        this.assignee = assignee;
    }

    public Employee getReviewer() {
        return reviewer;
    }

    public void setReviewer(Employee reviewer) {
        this.reviewer = reviewer;
    }
}
