package com.epam.rd.autotasks;

class CycleSwap {
    static void cycleSwap(int[] array) {
        if (isInvalidForCycleSwap(array)) {
            return;
        }

        int last = array[array.length - 1];
        System.arraycopy(array, 0, array, 1, array.length - 1);
        array[0] = last;
    }

    static void cycleSwap(int[] array, int shift) {
        if (isInvalidForCycleSwap(array)) {
            return;
        }

        shift %= array.length;
        int lastIndex = array.length - 1;
        reverse(array, 0, lastIndex);
        reverse(array, 0, shift - 1);
        reverse(array, shift, lastIndex);
    }

    private static boolean isInvalidForCycleSwap(int[] array) {
        return array == null || array.length < 2;
    }

    private static void reverse(int[] array, int left, int right) {
        while (left < right) {
            int temp = array[left];
            array[left] = array[right];
            array[right] = temp;

            left++;
            right--;
        }
    }
}
