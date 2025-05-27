package edu.epam.fop.spring.injection;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

@Component
public class SubscriptionComponent implements Subscription {
    private final Account user;
    private final Payment payment;
    private Period period;

    @Autowired
    public SubscriptionComponent(Account user, Payment payment, Period period) {
        this.user = user;
        this.payment = payment;
        this.period = period;
    }

    @Override
    public Account getUser() {
        return user;
    }

    @Override
    public Payment getPayment() {
        return payment;
    }

    @Override
    public Period getPeriod() {
        return period;
    }

    @Override
    public void setPeriod(Period period) {
        this.period = period;
    }
}
