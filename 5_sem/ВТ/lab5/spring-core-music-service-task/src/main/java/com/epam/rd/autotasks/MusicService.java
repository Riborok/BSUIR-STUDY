package com.epam.rd.autotasks;

import javax.annotation.PostConstruct;
import javax.annotation.PreDestroy;
import java.util.ArrayList;
import java.util.List;

public class MusicService {
    private List<Song> Songs = new ArrayList<>();

    @PostConstruct
    private void init() {
        for (int i = 0; i < 3; i++){
            Song temp = new Song("","","");
            Songs.add(temp);
        }
    }

    @PreDestroy
    private void destroy() {
        System.out.println("Music service is shutting down");
    }

    public MusicService() {
    }

    public List<Song> getSongs() {
        return Songs;
    }

    public void setSongs(List<Song> list) {
        this.Songs = list;
    }
}
