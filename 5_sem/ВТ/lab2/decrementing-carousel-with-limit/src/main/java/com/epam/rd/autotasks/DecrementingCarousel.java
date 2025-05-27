package com.epam.rd.autotasks;

public class DecrementingCarousel {
    protected final int[] elements;
    private final int capacity;
    private int currentIndex = 0;
    private boolean isRunning = false;

    public DecrementingCarousel(int capacity) {
        this.elements = new int[capacity];
        this.capacity = capacity;
    }

    public boolean addElement(int element){
        if (canAddElement(element)) {
            elements[currentIndex++] = element;
            return true;
        }
        return false;
    }

    private boolean canAddElement(int element) {
        return element > 0 && currentIndex < capacity && !isRunning;
    }

    public CarouselRun run(){
        if (isRunning) {
            return null;
        }
        isRunning = true;
        return new CarouselRun(this, currentIndex);
    }

    boolean decrementCurrentValue(int i){
        elements[i]--;
        return true;
    }

    int getElement(int i) {
        return elements[i];
    }

    int getElementCount() {
        return elements.length;
    }
}
