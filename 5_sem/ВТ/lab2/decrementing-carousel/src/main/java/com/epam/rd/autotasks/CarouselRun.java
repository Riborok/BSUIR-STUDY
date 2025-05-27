package com.epam.rd.autotasks;

public class CarouselRun {
    private final int[] elements;
    private int currentIndex = 0;
    private int size;

    public CarouselRun(int[] elements, int size) {
        this.elements = elements;
        this.size = size;
    }

    public int next() {
        if (isFinished()) {
            return -1;
        }

        moveToNextNonZeroElement();
        int value = getCurrentValue();
        decrementCurrentValue();
        updateSize();
        moveToNextPosition();
        return value;
    }

    private void moveToNextNonZeroElement() {
        while (elements[currentIndex] <= 0) {
            moveToNextPosition();
        }
    }

    private void moveToNextPosition() {
        currentIndex = (currentIndex + 1) % elements.length;
    }

    private int getCurrentValue() {
        return elements[currentIndex];
    }

    private void decrementCurrentValue() {
        elements[currentIndex]--;
    }

    private void updateSize() {
        if (elements[currentIndex] <= 0) {
            size--;
        }
    }

    public boolean isFinished() {
        return size <= 0;
    }

}
