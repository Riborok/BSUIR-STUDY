package com.epam.rd.autotasks;

public class Owner {
    String name;
    String taxNumber;

    public Owner() {
    }

    public Owner(String name, String taxNumber) {
        this.name = name;
        this.taxNumber = taxNumber;
    }

    public String getName(){
        return name;
    }

    public String getTaxNumber(){
        return taxNumber;
    }

    public void setName(String name) {
        this.name = name;
    }

    public void setTaxNumber(String taxNumber) {
        this.taxNumber = taxNumber;
    }
}
