package com.epam.rd.autotasks.segments;

class Point {
    private double x;
    private double y;

    public Point(final double x, final double y) {
        this.x = x;
        this.y = y;
    }

    public double getX() {
        return x;
    }

    public double getY() {
        return y;
    }

    public boolean equals(Point other) {
        return Double.compare(this.x, other.x) == 0 && Double.compare(this.y, other.y) == 0;
    }
}
