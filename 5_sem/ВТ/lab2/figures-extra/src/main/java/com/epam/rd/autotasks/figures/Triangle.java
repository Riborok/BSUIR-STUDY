package com.epam.rd.autotasks.figures;

class Triangle extends Figure {
    private final Point a;
    private final Point b;
    private final Point c;

    public Triangle(Point a, Point b, Point c) {
        this.a = a;
        this.b = b;
        this.c = c;

        if (isInvalid()) {
            throw new IllegalArgumentException();
        }
    }

    private boolean isInvalid() {
        return a == null || b == null || c == null || area() == 0;
    }

    double area() {
        return 0.5 * Math.abs(Point.calculateCrossProduct(a, b, c));
    }

    @Override
    public Point centroid() {
        double x = (a.getX() + b.getX() + c.getX()) / 3;
        double y = (a.getY() + b.getY() + c.getY()) / 3;
        return new Point(x, y);
    }

    @Override
    public boolean isTheSame(Figure figure) {
        if (!(figure instanceof Triangle)) {
            return false;
        }

        Triangle other = (Triangle) figure;
        return a.equals(other.a) && b.equals(other.b) && c.equals(other.c);
    }
}
