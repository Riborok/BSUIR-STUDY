package com.epam.rd.autotasks.config;

import com.epam.rd.autotasks.Singer;
import com.epam.rd.autotasks.Song;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Scope;

import java.util.Random;

@Configuration
public class AppConfig {
    final private Random rand = new Random();

    @Bean
    @Scope("prototype")
    public Song song(){
        return new Song(String.valueOf(rand.nextInt()));
    }

    @Bean
    @Scope("singleton")
    public Singer singer(){
        return new Singer("Elton John");
    }
}
