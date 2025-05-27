package com.epam.rd.autotasks;

public class CarouselRun {
    private final DecrementingCarousel carousel;
    private int currentIndex = 0;
    private int size;

    public CarouselRun(DecrementingCarousel carousel, int size) {
        this.carousel = carousel;
        this.size = size;
    }

    public int next() {
        if (isFinished()) {
            return -1;
        }

        moveToNextNonZeroElement();
        int value = getCurrentValue();
        carousel.decrementCurrentValue(currentIndex);
        updateSize();
        moveToNextPosition();
        return value;
    }

    private void moveToNextNonZeroElement() {
        while (carousel.getElement(currentIndex) <= 0) {
            moveToNextPosition();
        }
    }

    private void moveToNextPosition() {
        currentIndex = (currentIndex + 1) % carousel.getElementCount();
    }

    private int getCurrentValue() {
        return carousel.getElement(currentIndex);
    }

    private void updateSize() {
        if (carousel.getElement(currentIndex) <= 0) {
            size--;
        }
    }

    public boolean isFinished() {
        return size <= 0;
    }

}
