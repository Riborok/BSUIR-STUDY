package com.epam.rd.autotasks.figures;

public class DoubleComparer {
    private static final double EPSILON = 0.0001;

    public static boolean areEqual(double a, double b) {
        return Math.abs(a - b) < EPSILON;
    }
}