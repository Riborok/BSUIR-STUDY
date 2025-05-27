package com.epam.rd.autotasks.words;

import java.util.Arrays;

public class StringUtil {
    public static int countEqualIgnoreCaseAndSpaces(String[] words, String sample) {
        if (words == null || words.length == 0 || sample == null || sample.isEmpty()) {
            return 0;
        }

        sample = sample.toLowerCase();
        sample = sample.trim();
        int counter = 0;
        for (String word : words) {

            if (word == null || word.isEmpty()){
                continue;
            }
            String temp = word.toLowerCase();
            temp = temp.trim();
            int i = 0;
            int j = 0;
            if (temp.length() - i != sample.length()){
                continue;
            }
            boolean isEqual = true;
            while (j < sample.length()){
                if (temp.charAt(i++) != sample.charAt(j++)){
                    isEqual = false;
                    break;
                }
            }
            if (isEqual){counter++;}
        }
        return counter;
    }

    public static String[] splitWords(String text) {
        if (text == null || text.trim().isEmpty()) {
            return null;
        }
        String regex = "[,.;:?!\\s]+";
        text = text.replaceAll(regex, " ").trim();
        String[] res = text.split("\\s+");

        if (res.length == 0 || res.length == 1 && res[0].isEmpty()) {
            return null;
        }
        return res;
    }

    public static String convertPath(String path, boolean toWin) {
        if (path == null || path.isEmpty()) {return null;}
        if (path.lastIndexOf('~') > 0 || path.contains("C:")  && path.contains("/")||
                path.contains("~") && path.contains("\\") || path.lastIndexOf("C:") > 0 || path.contains("/") && path.contains("\\")) {
            return null;
        }

        if (toWin){
            if (path.startsWith("/"))
            {
                path = path.replaceFirst("/", "C:\\\\");
            }
            path = path.replace("/","\\");
            path = path.replace( "~","C:\\User");
        } else {
            path = path.replace("C:\\User", "~");
            if (path.startsWith("C:\\"))
            {
                path = path.replaceFirst("C:\\\\", "/");
            }
            path = path.replace("\\","/");

        }
        return path;
    }

    public static String joinWords(String[] words) {
        if (words == null || words.length == 0) {
            return null;
        }
        StringBuilder sb = new StringBuilder();
        sb.append("[");
        boolean onlyEmpty = true;
        for (String word : words) {
            if (word.isEmpty()) {
                continue;
            }
            if (!onlyEmpty) sb.append(", ");
            sb.append(word);
            onlyEmpty = false;
        }
        sb.append("]");
        return onlyEmpty ? null : sb.toString();
    }

    public static void main(String[] args) {
        System.out.println("Test 1: countEqualIgnoreCaseAndSpaces");
        String[] words = new String[]{" WordS    \t", "words", "w0rds", "WOR  DS", };
        String sample = "words   ";
        int countResult = countEqualIgnoreCaseAndSpaces(words, sample);
        System.out.println("Result: " + countResult);
        int expectedCount = 2;
        System.out.println("Must be: " + expectedCount);

        System.out.println("Test 2: splitWords");
        String text = "   ,, first, second!!!! third";
        String[] splitResult = splitWords(text);
        System.out.println("Result : " + Arrays.toString(splitResult));
        String[] expectedSplit = new String[]{"first", "second", "third"};
        System.out.println("Must be: " + Arrays.toString(expectedSplit));

        System.out.println("Test 3: convertPath");
        String unixPath = "/some/unix/path";
        String convertResult = convertPath(unixPath, true);
        System.out.println("Result: " + convertResult);
        String expectedWinPath = "C:\\some\\unix\\path";
        System.out.println("Must be: " + expectedWinPath);

        System.out.println("Test 4: joinWords");
        String[] toJoin = new String[]{"go", "with", "the", "", "FLOW"};
        String joinResult = joinWords(toJoin);
        System.out.println("Result: " + joinResult);
        String expectedJoin = "[go, with, the, FLOW]";
        System.out.println("Must be: " + expectedJoin);
    }
}