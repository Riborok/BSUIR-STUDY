package com.epam.rd.autotasks.config;

import com.epam.rd.autotasks.Owner;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class OwnerConfig {
    @Bean
    public Owner owner(){
        return new Owner("John Doe", "19671223-0000");
    }
}
