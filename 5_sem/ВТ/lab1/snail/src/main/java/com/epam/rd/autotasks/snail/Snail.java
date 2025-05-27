package com.epam.rd.autotasks.snail;

import java.util.Scanner;

public class Snail
{
    public static void main(String[] args)
    {
        //Write a program that reads a,b and h (line by lyne in this order) and prints
        //the number of days for which the snail reach the top of the tree.
        //a - feet that snail travels up each day, b - feet that slides down each night, h - height of the tree
        Scanner scanner = new Scanner(System.in);
        int a = scanner.nextInt();
        int b = scanner.nextInt();
        int h = scanner.nextInt();

        if (a >= h) {
            System.out.println(1);
            return;
        }

        int c = a - b; // distance per day

        if (c <= 0) {
            System.out.println("Impossible");
            return;
        }

        int days = (h - b + c - 1) / c;
        System.out.println(days); // c - 1 for rounding up.
    }
}
