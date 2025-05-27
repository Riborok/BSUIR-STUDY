package com.epam.rd.autotasks.figures;

class Quadrilateral extends Figure {
    private final Point a;
    private final Point b;
    private final Point c;
    private final Point d;

    public Quadrilateral(Point a, Point b, Point c, Point d) {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;

        tryCreateTriangles();
        if (!isConvex()) {
            throw new IllegalArgumentException();
        }
    }

    private void tryCreateTriangles() {
        new Triangle(a, b, c);
        new Triangle(a, b, d);
        new Triangle(a, c, d);
        new Triangle(b, c, d);
    }

    private boolean isConvex() {
        Point[] points = {a, b, c, d};
        boolean initialSign = determineInitialSign(points);

        for (int i = 1; i < points.length; i++) {
            if (!isConsistentSign(points, i, initialSign)) {
                return false;
            }
        }

        return true;
    }

    private boolean determineInitialSign(Point[] points) {
        double crossProduct = Point.calculateCrossProduct(points[0], points[1], points[2]);
        return crossProduct > 0;
    }

    private boolean isConsistentSign(Point[] points, int index, boolean initialSign) {
        Point p1 = points[index];
        Point p2 = points[(index + 1) % points.length];
        Point p3 = points[(index + 2) % points.length];

        double crossProduct = Point.calculateCrossProduct(p1, p2, p3);
        return (crossProduct > 0) == initialSign;
    }

    @Override
    public Point centroid() {
        Triangle triangle1 = new Triangle(a, b, c);
        Triangle triangle2 = new Triangle(a, c, d);

        double area1 = triangle1.area();
        double area2 = triangle2.area();

        Point centroid1 = triangle1.centroid();
        Point centroid2 = triangle2.centroid();

        return calculateWeightedCentroid(centroid1, area1, centroid2, area2);
    }

    private Point calculateWeightedCentroid(Point centroid1, double area1, Point centroid2, double area2) {
        double totalArea = area1 + area2;
        double centroidX = (centroid1.getX() * area1 + centroid2.getX() * area2) / totalArea;
        double centroidY = (centroid1.getY() * area1 + centroid2.getY() * area2) / totalArea;

        return new Point(centroidX, centroidY);
    }

    @Override
    public boolean isTheSame(Figure figure) {
        if (!(figure instanceof Quadrilateral)) {
            return false;
        }

        Quadrilateral other = (Quadrilateral) figure;
        return hasMatchingOrder(other) || hasMatchingReverseOrder(other);
    }

    private boolean hasMatchingOrder(Quadrilateral other) {
        Point[] thisPoints = getPointsArray();
        Point[] otherPoints = other.getPointsArray();

        for (int offset = 0; offset < 4; offset++) {
            if (matchesInOrder(thisPoints, otherPoints, offset)) {
                return true;
            }
        }
        return false;
    }

    private Point[] getPointsArray() {
        return new Point[]{a, b, c, d};
    }

    private boolean matchesInOrder(Point[] thisPoints, Point[] otherPoints, int offset) {
        for (int i = 0; i < 4; i++) {
            if (!thisPoints[i].equals(otherPoints[(i + offset) % 4])) {
                return false;
            }
        }
        return true;
    }

    private boolean hasMatchingReverseOrder(Quadrilateral other) {
        Point[] thisPoints = getPointsArray();
        Point[] otherPoints = other.getPointsArray();

        for (int offset = 0; offset < 4; offset++) {
            if (matchesInReverseOrder(thisPoints, otherPoints, offset)) {
                return true;
            }
        }
        return false;
    }

    private boolean matchesInReverseOrder(Point[] thisPoints, Point[] otherPoints, int offset) {
        for (int i = 0; i < 4; i++) {
            if (!thisPoints[i].equals(otherPoints[(offset - i + 4) % 4])) {
                return false;
            }
        }
        return true;
    }
}
