package com.epam.rd.autotasks;

import java.util.ArrayList;
import java.util.List;

public class TaskCarousel {
    private final List<Task> tasks;
    private final int capacity;
    private int currentIndex = -1;

    public TaskCarousel(int capacity) {
        this.tasks = new ArrayList<>(capacity);
        this.capacity = capacity;
    }

    public boolean addTask(Task task) {
        if (task == null || isFull() || task.isFinished()) {
            return false;
        }

        tasks.add(task);
        return true;
    }

    public boolean execute() {
        if (isEmpty()) {
            return false;
        }

        updateCurrentIndex();
        executeCurrentTask();
        removeCurrentTaskIfFinished();
        return true;
    }

    private void updateCurrentIndex() {
        currentIndex = (currentIndex + 1) % tasks.size();
    }

    private void executeCurrentTask() {
        getCurrentTask().execute();
    }

    private Task getCurrentTask() {
        return tasks.get(currentIndex);
    }

    private void removeCurrentTaskIfFinished() {
        if (getCurrentTask().isFinished()) {
            removeCurrentTask();
        }
    }

    private void removeCurrentTask() {
        tasks.remove(currentIndex);
        currentIndex--;
    }

    public boolean isFull() {
        return tasks.size() == capacity;
    }

    public boolean isEmpty() {
        return tasks.isEmpty();
    }

}
