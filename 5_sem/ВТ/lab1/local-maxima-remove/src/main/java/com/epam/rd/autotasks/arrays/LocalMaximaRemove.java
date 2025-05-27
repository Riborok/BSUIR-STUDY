package com.epam.rd.autotasks.arrays;

import java.util.Arrays;

public class LocalMaximaRemove {

    public static void main(String[] args) {
        int[] array = new int[]{18, 1, 3, 6, 7, -5};

        System.out.println(Arrays.toString(removeLocalMaxima(array)));
    }

    public static int[] removeLocalMaxima(int[] array){
        int count = 0;
        for (int i = 0; i < array.length; i++) {
            if (isNotLocalMaximum(array, i)) {
                count++;
            }
        }

        int[] result = new int[count];
        int index = 0;
        for (int i = 0; i < array.length; i++) {
            if (isNotLocalMaximum(array, i)) {
                result[index++] = array[i];
            }
        }
        return result;
    }

    private static boolean isNotLocalMaximum(int[] array, int index) {
        return !isLocalMaximum(array, index);
    }

    private static boolean isLocalMaximum(int[] array, int index) {
        return (index == 0 || array[index] > array[index - 1]) &&
                (index == array.length - 1 || array[index] > array[index + 1]);
    }
}
