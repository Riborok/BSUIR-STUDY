package com.epam.rd.autotasks;

import java.util.Scanner;

public class Average {
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);
        // Use Scanner methods to read input

        int number = scanner.nextInt();
        int sum = 0;
        int count = 0;
        while (number != 0) {
            sum += number;
            count++;
            number = scanner.nextInt();
        }

        int average = sum / count;
        System.out.println(average);
    }
}