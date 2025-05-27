package com.epam.rd.autotasks;

public class HalvingCarousel extends DecrementingCarousel {

    public HalvingCarousel(final int capacity) {
        super(capacity);
    }

    @Override
    void decrementCurrentValue(int i) {
        elements[i] /= 2;
    }
}
