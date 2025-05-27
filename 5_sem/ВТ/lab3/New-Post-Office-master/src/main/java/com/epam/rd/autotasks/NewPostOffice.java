package com.epam.rd.autotasks;

import java.math.BigDecimal;
import java.math.MathContext;
import java.util.ArrayList;
import java.util.Collection;

public class NewPostOffice {
    private final Collection<Box> listBox;
    private static final int COST_KILOGRAM = 5;
    private static final int COST_CUBIC_METER = 100;
    private static final double COEFFICIENT = 0.5;

    public NewPostOffice() {
        listBox = new ArrayList<>();
    }

    public Collection<Box> getListBox() {
        return (Collection<Box>) ((ArrayList<Box>) listBox).clone();
    }

    static BigDecimal calculateCostOfBox(double weight, double volume, int value) {
        BigDecimal costWeight = BigDecimal.valueOf(weight)
                .multiply(BigDecimal.valueOf(COST_KILOGRAM), MathContext.DECIMAL64);
        BigDecimal costVolume = BigDecimal.valueOf(volume)
                .multiply(BigDecimal.valueOf(COST_CUBIC_METER), MathContext.DECIMAL64);
        return costVolume.add(costWeight)
                .add(BigDecimal.valueOf(COEFFICIENT * value), MathContext.DECIMAL64);
    }

    // implements student
    public boolean addBox(String addresser, String recipient, double weight, double volume, int value) {
        validateBoxParameters(addresser, recipient, weight, volume, value);
        Box temp = new Box(addresser, recipient, weight, volume);
        temp.setCost(calculateCostOfBox(weight, volume, value));
        listBox.add(temp);
        return true;
    }

    private void validateBoxParameters(String addresser, String recipient, double weight, double volume, int value) {
        if (addresser == null || recipient == null ||
                addresser.trim().isEmpty() || recipient.trim().isEmpty() ||
                weight > 20.0 || weight < 0.5 || volume <= 0 || volume > 0.25 || value <= 0) {
            throw new IllegalArgumentException("Invalid box parameters.");
        }
    }

    // implements student
    public Collection<Box> deliveryBoxToRecipient(String recipient) {
        Collection <Box> deliveryBox = new ArrayList<>();
        for (Box box : getListBox()) {
            if (box.getRecipient().equals(recipient)) {
                listBox.remove(box);
                deliveryBox.add(box);
            }
        }
        return deliveryBox;
    }

    public void declineCostOfBox(double percent){
        for (Box box : listBox) {
            BigDecimal newCost = box.getCost()
                    .multiply(BigDecimal.valueOf(100 - percent), MathContext.DECIMAL64)
                    .divide(BigDecimal.valueOf(100), MathContext.DECIMAL64);
            box.setCost(newCost);
        }
    }

}
