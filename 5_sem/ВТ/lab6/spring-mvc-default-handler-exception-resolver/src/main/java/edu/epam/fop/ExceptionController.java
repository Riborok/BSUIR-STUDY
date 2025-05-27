package edu.epam.fop;

import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestMapping;

@Controller
public class ExceptionController {

    @RequestMapping("/trigger-number-format")
    public String triggerNumberFormatException() {
        Integer.parseInt("InvalidNumber");
        return "error";
    }

    @RequestMapping("/trigger-null-pointer")
    public String triggerNullPointerException() {
        String nullString = null;
        nullString.length();
        return "error";
    }
}