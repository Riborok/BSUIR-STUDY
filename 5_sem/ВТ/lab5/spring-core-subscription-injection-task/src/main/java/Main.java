import edu.epam.fop.spring.injection.Subscription;
import org.springframework.context.annotation.AnnotationConfigApplicationContext;
import org.springframework.context.annotation.ComponentScan;

@ComponentScan(basePackages = "edu.epam.fop.spring.injection")
public class Main {
    public static void main(String[] args) {
        AnnotationConfigApplicationContext context = new AnnotationConfigApplicationContext(Main.class);

        Subscription subscription = context.getBean(Subscription.class);

        System.out.println("Account: " + subscription.getUser().toString());
        System.out.println("Payment: " + subscription.getPayment().toString());
        System.out.println("Period: " + subscription.getPeriod().toString());

        context.close();
    }
}