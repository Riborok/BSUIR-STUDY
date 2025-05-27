package com.epam.rd.autotasks;

public class Car {
    String model;
    String year;
    Owner owner;

    public Car() {
    }

    public Car(String model, String year, Owner owner) {
        this.model = model;
        this.year = year;
        this.owner = owner;
    }

    public String getModel() {
        return model;
    }

    public Owner getOwner() {
        return owner;
    }

    public String getYear() {
        return year;
    }

    public void setModel(String model) {
        this.model = model;
    }

    public void setOwner(Owner owner) {
        this.owner = owner;
    }

    public void setYear(String year) {
        this.year = year;
    }
}
