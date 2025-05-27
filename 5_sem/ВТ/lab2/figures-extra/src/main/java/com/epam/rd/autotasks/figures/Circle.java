package com.epam.rd.autotasks.figures;

class Circle extends Figure {
    private final double radius;
    private final Point center;

    public Circle(Point center, double radius) {
        if (center == null || radius <= 0) {
            throw new IllegalArgumentException();
        }
        this.radius = radius;
        this.center = center;
    }

    @Override
    public Point centroid() {
        return center;
    }

    @Override
    public boolean isTheSame(Figure figure) {
        if (!(figure instanceof Circle)) {
            return false;
        }

        Circle other = (Circle) figure;
        return center.equals(other.center) && DoubleComparer.areEqual(radius, other.radius);
    }
}
