package edu.epam.fop.spring.injection;

import org.springframework.stereotype.Component;

@Component
public class PaymentComponent implements Payment {
    private final long amount;
    private final String type;

    public PaymentComponent() {
        this.amount = 42;
        this.type = "Test Type";
    }

    @Override
    public long getAmount() {
        return amount;
    }

    @Override
    public String getType() {
        return type;
    }

    @Override
    public String toString() {
        return String.format("PaymentComponent{amount=%d, type='%s'}", amount, type);
    }
}
