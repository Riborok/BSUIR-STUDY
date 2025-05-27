package com.epam.rd.autotasks.config;

import com.epam.rd.autotasks.Song;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.PropertySource;

@Configuration
@PropertySource("classpath:application.properties")
public class SongConfig {

    @Value ("${song.title}")
    private String title;

    @Value ("${song.artist}")
    private String artist;

    @Value ("${song.year}")
    private String year;

    @Bean
    public Song song() {
        return new Song(title, artist, year);
    }
}
