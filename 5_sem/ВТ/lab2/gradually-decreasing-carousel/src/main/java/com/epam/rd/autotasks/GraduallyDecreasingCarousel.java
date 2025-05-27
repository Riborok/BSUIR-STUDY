package com.epam.rd.autotasks;

public class GraduallyDecreasingCarousel extends DecrementingCarousel{
    public final int[] decrementSteps;

    public GraduallyDecreasingCarousel(final int capacity) {
        super(capacity);
        decrementSteps = new int[capacity];
    }

    @Override
    void decrementCurrentValue(int i) {
        elements[i] -= ++decrementSteps[i];
    }
}
