package com.epam.rd.autotasks.figures;

class Circle extends Figure{
    private final double radius;
    private final Point center;

    public Circle(Point center, double radius) {
        this.radius = radius;
        this.center = center;
    }

    @Override
    public double area() {
        return Math.PI * radius * radius;
    }

    @Override
    public Point leftmostPoint() {
        return new Point(center.getX() - radius, center.getY());
    }

    @Override
    public String pointsToString() {
        return String.format("(%s,%s)", center.getX(), center.getY());
    }

    @Override
    public String toString() {
        return String.format("Circle[%s%s]", pointsToString(), radius);
    }
}
