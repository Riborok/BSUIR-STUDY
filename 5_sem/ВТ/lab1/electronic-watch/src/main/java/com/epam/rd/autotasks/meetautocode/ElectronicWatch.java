package com.epam.rd.autotasks.meetautocode;

import java.util.Scanner;

public class ElectronicWatch {
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);
        int totalSeconds = scanner.nextInt();

        int seconds = totalSeconds % 60;
        int minutes = (totalSeconds % 3600) / 60;
        int hours = (totalSeconds / 3600) % 24;

        System.out.printf("%d:%02d:%02d", hours, minutes, seconds);
    }
}
