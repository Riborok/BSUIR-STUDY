package com.epam.rd.autotasks.config;

import com.epam.rd.autotasks.Employee;
import com.epam.rd.autotasks.Task;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class AppConfig {

    @Bean
    @Qualifier("Assignee")
    public Employee junior() {
        return new Employee("John Doe", "Junior Software Engineer");
    }

    @Bean
    @Qualifier("Reviewer")
    public Employee senior() {
        return new Employee("Emily Brown", "Senior Software Engineer");
    }

    @Bean
    public Task task(@Qualifier("Assignee") Employee assignee, @Qualifier("Reviewer") Employee reviewer) {
        return new Task("New feature", assignee, reviewer);
    }
}
