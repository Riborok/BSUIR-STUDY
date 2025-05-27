package com.epam.rd.autotasks.sprintplanning;

import com.epam.rd.autotasks.sprintplanning.tickets.Bug;
import com.epam.rd.autotasks.sprintplanning.tickets.Ticket;
import com.epam.rd.autotasks.sprintplanning.tickets.UserStory;

import java.util.Arrays;

public class Sprint {
    private final Ticket[] tickets;
    private final int capacity;
    private final int ticketsLimit;
    private int totalEstimate;
    private int ticketIndex;

    public Sprint(int capacity, int ticketsLimit) {
        this.capacity = capacity;
        this.ticketsLimit = ticketsLimit;
        this.tickets = new Ticket[ticketsLimit];
    }

    public boolean addUserStory(UserStory userStory) {
        if (isNotAddable(userStory) || !areDependenciesSatisfied(userStory)) {
            return false;
        }

        tickets[ticketIndex++] = userStory;
        totalEstimate += userStory.getEstimate();
        return true;
    }

    private boolean isNotAddable(Ticket ticket) {
        return ticket == null || ticket.isCompleted() || isOverCapacity(ticket) || isLimitReached();
    }

    private boolean isOverCapacity(Ticket ticket) {
        return ticket.getEstimate() + totalEstimate > capacity;
    }

    private boolean isLimitReached() {
        return ticketIndex >= ticketsLimit;
    }

    public boolean areDependenciesSatisfied(UserStory userStory) {
        for (UserStory dependency : userStory.getDependencies()) {
            if (!dependency.isCompleted() && !isTicketFound(dependency)) {
                return false;
            }
        }
        return true;
    }

    private boolean isTicketFound(UserStory story) {
        for (Ticket ticket : tickets) {
            if (story == ticket) {
                return true;
            }
        }
        return false;
    }

    public boolean addBug(Bug bugReport) {
        if (isNotAddable(bugReport)) {
            return false;
        }

        tickets[ticketIndex++] = bugReport;
        totalEstimate += bugReport.getEstimate();
        return true;
    }

    public Ticket[] getTickets() {
        return Arrays.copyOf(tickets, ticketIndex);
    }

    public int getTotalEstimate() {
        return totalEstimate;
    }
}
