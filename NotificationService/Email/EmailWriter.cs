namespace NotificationService.Email;

public class EmailWriter
{
    public void WriteOrderSubmitted(string to, IEmailService emailService)
    {
        var subject = "Order submitted";
        var text = "Thank you for your purchase! We are collecting your order.";
        emailService.Send(to, subject, text);
    }

    public void WriteOrderUnderway(string to, IEmailService emailService)
    {
        var subject = "Order underway";
        var text = "We gave your order to the transport company and it is now on its way toward you!";
        emailService.Send(to, subject, text);
    }

    public void WriteOrderArrived(string to, IEmailService emailService)
    {
        var subject = "Order arrived";
        var text = "Your order has arrived, we hope that it is to your liking";
        emailService.Send(to, subject, text);
    }

    public void WriteOrderArrivedPaymentNeeded(string to, IEmailService emailService)
    {
        var subject = "Payment open";
        var text = "Your order has arrived, we hope that it is to your liking \n please pay.";
        emailService.Send(to, subject, text);
    }

    public void WriteTicketCreated(string to, IEmailService emailService)
    {
        var subject = "Ticket created";
        var text = "Your ticket has been submitted, one of our workers will pick it up in no time";
        emailService.Send(to, subject, text);
    }

    public void WriteTicketUpdated(string to, IEmailService emailService)
    {
        var subject = "Ticket updated";
        var text = "Your ticket status has been updated";
        emailService.Send(to, subject, text);
    }
}