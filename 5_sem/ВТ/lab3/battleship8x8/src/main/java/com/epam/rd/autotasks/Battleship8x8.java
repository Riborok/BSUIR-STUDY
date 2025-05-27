package com.epam.rd.autotasks;

public class Battleship8x8 {
    private final long ships;
    private long shots = 0L;

    public Battleship8x8(final long ships) {
        this.ships = ships;
    }

    public boolean shoot(String shot) {
        int position = getPositionFromShot(shot);
        shots |= (1L << position);
        return isHit(position);
    }

    private int getPositionFromShot(String shot) {
        int column = shot.charAt(0) - 'A';
        int row = shot.charAt(1) - '1';
        return row * 8 + column;
    }

    private boolean isHit(int position) {
        long shipPos = 1L << (64 - position - 1);
        return (ships & shipPos) != 0;
    }

    public String state() {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                appendCellState(sb, i, j);
            }
            sb.append("\n");
        }
        return sb.toString();
    }

    private void appendCellState(StringBuilder sb, int row, int col) {
        int pow = row * 8 + col;
        long shotPos = 1L << pow;
        long shipPos = 1L << (64 - pow - 1);

        if ((ships & shipPos) != 0) {
            sb.append((shots & shotPos) != 0 ? '☒' : '☐');
        } else {
            sb.append((shots & shotPos) != 0 ? '×' : '.');
        }
    }
}