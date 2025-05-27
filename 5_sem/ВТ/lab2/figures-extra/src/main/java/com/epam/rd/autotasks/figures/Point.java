package com.epam.rd.autotasks.figures;

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
        return DoubleComparer.areEqual(this.x, other.x) && DoubleComparer.areEqual(this.y, other.y);
    }

    public static double calculateCrossProduct(Point p1, Point p2, Point p3) {
        return (p2.getX() - p1.getX()) * (p3.getY() - p2.getY()) - (p2.getY() - p1.getY()) * (p3.getX() - p2.getX());
    }
}
