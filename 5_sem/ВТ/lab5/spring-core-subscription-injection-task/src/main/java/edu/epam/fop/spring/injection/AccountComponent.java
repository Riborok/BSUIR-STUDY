package edu.epam.fop.spring.injection;

import org.springframework.stereotype.Component;

@Component
public class AccountComponent implements Account {
    private final String name;

    public AccountComponent() {
        this.name = "Test";
    }

    @Override
    public String getName() {
        return name;
    }

    @Override
    public String toString() {
        return String.format("AccountComponent{name='%s'}", name);
    }
}
