package edu.epam.fop.controller;

import edu.epam.fop.model.FormData;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.PostMapping;

/** Add controller implementation here. */
@Controller
public class FormController {

    @GetMapping("/form")
    public String form(Model model) {
        model.addAttribute("formData", new FormData());
        return "formTemplate";
    }

    @PostMapping("/processForm")
    public String processForm(@ModelAttribute("formData") FormData formData, Model model) {
        model.addAttribute("formData", formData);
        return "resultTemplate";
    }
}
