package com.epam.rd.autotasks;

public class CountDownTask implements Task{
    private int value;
    private boolean isFinished;

    public CountDownTask(int value) {
        this.value = Math.max(0, value);
        isFinished = this.value == 0;
    }

    public int getValue() {
        return value;
    }

    @Override
    public void execute() {
        if (isFinished)
            return;
        isFinished = --value == 0;
    }

    @Override
    public boolean isFinished() {
        return isFinished;
    }
}
