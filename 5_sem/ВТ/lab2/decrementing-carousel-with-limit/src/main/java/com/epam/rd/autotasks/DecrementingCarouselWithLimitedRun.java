package com.epam.rd.autotasks;

public class DecrementingCarouselWithLimitedRun extends DecrementingCarousel{
    private int actionLimit;

    public DecrementingCarouselWithLimitedRun(final int capacity, final int actionLimit) {
        super(capacity);
        this.actionLimit = actionLimit;
    }

    @Override
    boolean decrementCurrentValue(int i){
        if (actionLimit <= 0) {
            return false;
        }
        elements[i]--;
        return --actionLimit > 0;
    }
}
