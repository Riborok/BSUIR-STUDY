package com.epam.rd.autotasks.config;

import com.epam.rd.autotasks.MusicService;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class MusicServiceConfig {
    @Bean
    public MusicService musicService() {
        return new MusicService();
    }
}
