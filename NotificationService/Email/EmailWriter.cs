namespace NotificationService.Email;

public class EmailWriter
{

    public EmailWriter()
    {
    }

    public void WriteOrderSubmitted(string to, IEmailService emailService)
    {
        string subject = "Order submitted";
        string text = "Thank you for your purchase! We are collecting your order.";
        emailService.Send(to, subject, text);
    }

    public void WriteOrderUnderway(string to, IEmailService emailService)
    {
        string subject = "Order underway";
        string text = "We gave your order to the transport company and it is now on its way toward you!";
        emailService.Send(to, subject, text);
    }

    public void WriteOrderArrived(string to, IEmailService emailService)
    {
        string subject = "Order arrived";
        string text = "Your order has arrived, we hope that it is to your liking";
        emailService.Send(to, subject, text);
    }

    public void WriteOrderArrivedPaymentNeeded(string to, IEmailService emailService)
    {
        string subject = "Payment open";
        string text = "Your order has arrived, we hope that it is to your liking \n please pay.";
        emailService.Send(to, subject, text);
    }

    public void WriteTicketCreated(string to, IEmailService emailService)
    {
        string subject = "Ticket created";
        string text = "Your ticket has been submitted, one of our workers will pick it up in no time";
        emailService.Send(to, subject, text);
    }

    public void WriteTicketUpdated(string to, IEmailService emailService)
    {
        string subject = "Ticket updated";
        string text = "Your ticket status has been updated";
        emailService.Send(to, subject, text);
    }
    
    
}