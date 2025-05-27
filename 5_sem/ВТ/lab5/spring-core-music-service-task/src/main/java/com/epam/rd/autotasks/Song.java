package com.epam.rd.autotasks;

public class Song {
    private String title;
    private String artist;
    private String Year;

    public Song() {
    }

    public Song(String title, String artist, String Year) {
        this.title = title;
        this.artist = artist;
        this.Year = Year;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getArtist() {
        return artist;
    }

    public void setArtist(String artist) {
        this.artist = artist;
    }

    public String getYear() {
        return Year;
    }

    public void setYear(String year) {
        Year = year;
    }
}
