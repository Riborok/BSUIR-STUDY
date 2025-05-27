package com.epam.rd.autotasks.triangle;

class Triangle {
    private final Point a;
    private final Point b;
    private final Point c;

    public Triangle(Point a, Point b, Point c) {
        this.a = a;
        this.b = b;
        this.c = c;

        if (!isValidTriangle()) {
            throw new IllegalArgumentException();
        }
    }

    private boolean isValidTriangle() {
        return area() != 0;
    }

    public double area() {
        double a11 = a.getX() - c.getX();
        double a12 = a.getY() - c.getY();
        double a21 = b.getX() - c.getX();
        double a22 = b.getY() - c.getY();
        return 0.5 * Math.abs(a11 * a22 - a12 * a21);
    }

    public Point centroid(){
        double x = (a.getX() + b.getX() + c.getX()) / 3.0;
        double y = (a.getY() + b.getY() + c.getY()) / 3.0;
        return new Point(x, y);
    }
}
