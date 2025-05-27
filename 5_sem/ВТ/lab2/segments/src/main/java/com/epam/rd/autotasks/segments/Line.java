package com.epam.rd.autotasks.segments;

public class Line {
    private final double k;
    private final double b;

    public Line(double k, double b) {
        this.k = k;
        this.b = b;
    }

    Point intersection(Line other) {
        if (this.k == other.k)
            return null;

        double x = (other.b - this.b) / (this.k - other.k);
        double y = this.k * x + this.b;
        return new Point(x, y);
    }
}