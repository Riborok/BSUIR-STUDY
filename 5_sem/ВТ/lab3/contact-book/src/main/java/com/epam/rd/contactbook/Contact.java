package com.epam.rd.contactbook;

public class Contact {
    private String name;
    private final ContactInfo[] contacts;
    private ContactInfo phoneNumber = null;
    private int emailCount = 0;
    private int socialCount = 0;

    private class NameContactInfo implements ContactInfo {
        @Override
        public String getTitle() {
            return "Name";
        }

        @Override
        public String getValue() {
            return name;
        }
    }

    public static class Email implements ContactInfo {
        private final String email;
        private static final String title = "Email";

        Email(String email) {
            this.email = email;
        }

        @Override
        public String getTitle() {
            return title;
        }

        @Override
        public String getValue() {
            return email;
        }
    }

    public static class Social implements ContactInfo {
        private final String social;
        private final String title;

        Social(String social, String title) {
            this.social = social;
            this.title = title;
        }

        @Override
        public String getTitle() {
            return title;
        }

        @Override
        public String getValue() {
            return social;
        }
    }

    private void AddEntry(ContactInfo contactInfo, int index) {
        int i = index;
        while (contacts[i] != null) {
            i++;
        }
        contacts[i] = contactInfo;
    }

    public Contact(String contactName) {
        name = contactName;
        contacts= new  ContactInfo[8];
    }

    public void rename(String newName) {
        if (newName != null && !newName.isEmpty()) {
            name = newName;
        }
    }

    public Email addEmail(String localPart, String domain) {
        if (emailCount >= 3){
            return null;
        }
        emailCount++;
        Email res = new Email(localPart + "@" + domain);
        AddEntry(res, 0);
        return res;
    }


    public Email addEpamEmail(String firstname, String lastname) {
        if (emailCount >= 3){
            return null;
        }
        emailCount++;
        Email res = new Email(firstname + "_" + lastname + "@epam.com"){
            @Override
            public String getTitle() {
                return "Epam Email";
            }
        };
        AddEntry(res, 0);
        return res;
    }

    public ContactInfo addPhoneNumber(int code, String number) {
        if (phoneNumber != null || number == null || number.isEmpty()) {
            return null;
        }
        phoneNumber = new ContactInfo() {
            @Override
            public String getTitle() {
                return "Tel";
            }
            @Override
            public String getValue() {
                return "+" + code + " " + number;
            }
        };
        return phoneNumber;
    }

    public Social addTwitter(String twitterId) {
        if (socialCount >= 5){
            return null;
        }
        socialCount++;
        Social res = new Social(twitterId, "Twitter");
        AddEntry(res, 3);
        return res;
    }

    public Social addInstagram(String instagramId) {
        if (socialCount >= 5){
            return null;
        }
        socialCount++;
        Social res =  new Social(instagramId, "Instagram");
        AddEntry(res, 3);
        return res;
    }

    public Social addSocialMedia(String title, String id) {
        if (socialCount >= 5){
            return null;
        }
        socialCount++;
        Social res =  new Social(id, title);
        AddEntry(res, 3);
        return res;
    }

    public ContactInfo[] getInfo() {
        int arrSize = 1 + socialCount + emailCount + (phoneNumber == null ? 0 : 1);
        ContactInfo[] res = new ContactInfo[arrSize];
        int i = 0;
        res[i++] = new NameContactInfo();
        if (phoneNumber != null) {
            res[i++] = phoneNumber;
        }
        for (ContactInfo contact : contacts) {
            if (contact != null) {
                res[i++] = contact;
            }
        }
        return res;
    }

}
