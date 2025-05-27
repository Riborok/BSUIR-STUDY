package com.epam.rd.autotasks.config;

import com.epam.rd.autotasks.Car;
import com.epam.rd.autotasks.Owner;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Import;

@Configuration
@Import(OwnerConfig.class)
public class CarConfig {

    @Bean
    public Car car(Owner owner) {
        return new Car("Tesla Model X", "2022", owner);
    }
}
