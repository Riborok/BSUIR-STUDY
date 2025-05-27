package edu.epam.fop.spring.injection;

import org.springframework.stereotype.Component;

import java.time.Duration;
import java.time.LocalDate;

@Component
public class PeriodComponent implements Period {
    private final Duration duration;
    private final LocalDate date;

    public PeriodComponent() {
        this.duration = Duration.ofDays(42);
        this.date = LocalDate.now();
    }

    @Override
    public Duration paymentPeriod() {
        return Duration.ofDays(duration.toDays());
    }

    @Override
    public LocalDate endDate() {
        return date.plusDays(duration.toDays());
    }

    @Override
    public String toString() {
        return String.format("PeriodComponent{duration=%s, date=%s}", duration, date);
    }
}
