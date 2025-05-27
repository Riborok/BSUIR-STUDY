package com.epam.rd.autotasks.segments;

import static java.lang.Math.abs;
import static java.lang.Math.sqrt;
import static java.lang.StrictMath.pow;

class Segment {
    private final Point start;
    private final Point end;

    public Segment(Point start, Point end) {
        if (start.equals(end)){
            throw new IllegalArgumentException();
        }

        this.start = start;
        this.end = end;
    }

    double length() {
        double dx = end.getX() - start.getX();
        double dy = end.getY() - start.getY();
        return Math.sqrt(dx * dx + dy * dy);
    }

    Point middle() {
        double midX = (end.getX() + start.getX()) / 2;
        double midY = (end.getY() + start.getY()) / 2;
        return new Point(midX, midY);
    }

    Point intersection(Segment another) {
        Line thisLine = this.toLine();
        Line anotherLine = another.toLine();

        Point lineIntersection = thisLine.intersection(anotherLine);

        return isValidIntersection(lineIntersection, this, another) ? lineIntersection : null;
    }

    private Line toLine() {
        double x1 = start.getX();
        double y1 = start.getY();
        double x2 = end.getX();
        double y2 = end.getY();

        double k = (y2 - y1) / (x2 - x1);
        double b = (x2 * y1 - x1 * y2) / (x2 - x1);
        return new Line(k, b);
    }

    private static boolean isValidIntersection(Point intersection, Segment segment1, Segment segment2) {
        return intersection != null && segment1.containsPoint(intersection) && segment2.containsPoint(intersection);
    }

    private boolean containsPoint(Point point) {
        double px = point.getX();
        double py = point.getY();

        double x1 = start.getX();
        double y1 = start.getY();
        double x2 = end.getX();
        double y2 = end.getY();

        return (Math.min(x1, x2) <= px && Math.max(x1, x2) >= px) &&
                (Math.min(y1, y2) <= py && Math.max(y1, y2) >= py);
    }
}
