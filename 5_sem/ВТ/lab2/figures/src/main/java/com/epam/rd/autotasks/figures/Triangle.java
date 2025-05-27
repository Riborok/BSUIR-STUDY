package com.epam.rd.autotasks.figures;

class Triangle extends Figure {
    private final Point a;
    private final Point b;
    private final Point c;

    public Triangle(Point a, Point b, Point c) {
        this.a = a;
        this.b = b;
        this.c = c;
    }

    @Override
    public double area() {
        double a11 = a.getX() - c.getX();
        double a12 = a.getY() - c.getY();
        double a21 = b.getX() - c.getX();
        double a22 = b.getY() - c.getY();
        return 0.5 * Math.abs(a11 * a22 - a12 * a21);
    }

    @Override
    public Point leftmostPoint() {
        Point leftmost = a;
        if (b.getX() < leftmost.getX()) {
            leftmost = b;
        }
        if (c.getX() < leftmost.getX()) {
            leftmost = c;
        }
        return leftmost;
    }

    @Override
    public String pointsToString() {
        return String.format("(%s,%s)(%s,%s)(%s,%s)", a.getX(), a.getY(), b.getX(), b.getY(), c.getX(), c.getY());
    }
}
