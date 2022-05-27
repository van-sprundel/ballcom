using BallCore.Enums;
using BallCore.Events;
using BallCore.RabbitMq;
using NotificationService.DataAccess;
using NotificationService.Email;
using NotificationService.Models;
using RabbitMQ.Client;

namespace NotificationService;

public class NotificationMessageReceiver : MessageReceiver
{
    private readonly NotificationServiceDbContext _dbContext;
    private readonly IEmailService _emailService;
    private readonly EmailWriter _emailWriter;

    public NotificationMessageReceiver(IConnection connection, NotificationServiceDbContext dbContext,
        IEmailService emailService) :
        base(connection, new[] { "notifications" })
    {
        _dbContext = dbContext;
        _emailWriter = new EmailWriter();
        _emailService = emailService;
    }

    protected override Task HandleMessage(IEvent e)
    {
        Console.WriteLine("Received message");
        if (e is DomainEvent de)
            switch (de.Payload)
            {
                case Order c:
                {
                    Console.WriteLine(
                        $"Received ex: {de.UseExchange} {de.Type} message ({de.Name}) from {de.Destination} : {c.CustomerId}");
                    if (de.Type == EventType.Created)
                    {
                        Console.WriteLine("Order Created");
                        break;
                    }


                    if (de.Type == EventType.Updated)
                    {
                        Console.WriteLine("Sending Email about order updated");
                        var customer = _dbContext.Customers.FirstOrDefault(cu => cu.CustomerId == c.CustomerId);
                        if (customer == null || customer.Email == null)
                        {
                            Console.WriteLine("customer does not exist.");
                            break;
                        }

                        // Send email order submitted/
                        if (c.StatusProcess == StatusProcess.Collecting)
                        {
                            _emailWriter.WriteOrderSubmitted(customer.Email, _emailService);
                            break;
                        }

                        // Send email order underway.
                        if (c.StatusProcess == StatusProcess.Underway)
                        {
                            _emailWriter.WriteOrderUnderway(customer.Email, _emailService);
                            break;
                        }


                        // Send email order arrived
                        if (c.StatusProcess == StatusProcess.Arrived)
                        {
                            if (c.IsPaid == true)
                                _emailWriter.WriteOrderArrived(customer.Email, _emailService);
                            else
                                _emailWriter.WriteOrderArrivedPaymentNeeded(customer.Email, _emailService);
                        }
                    }

                    break;
                }
                case Ticket c:
                {
                    Console.WriteLine(
                        $"Received ex: {de.UseExchange} {de.Type} message ({de.Name}) from {de.Destination} : {c.CustomerId}");
                    var customer = _dbContext.Customers.FirstOrDefault(cu => cu.CustomerId == c.CustomerId);
                    if (customer == null || customer.Email == null)
                    {
                        Console.WriteLine("customer does not exist.");
                        break;
                    }

                    if (de.Type == EventType.Created)
                    {
                        Console.WriteLine("Sending Email about ticket created");
                        _emailWriter.WriteTicketCreated(customer.Email, _emailService);
                        break;
                    }

                    if (de.Type == EventType.Updated)
                    {
                        Console.WriteLine("Sending Email about ticket created");
                        _emailWriter.WriteTicketUpdated(customer.Email, _emailService);
                    }

                    break;
                }
                case Customer c:
                {
                    Console.WriteLine(
                        $"Received ex: {de.UseExchange} {de.Type} message ({de.Name}) from {de.Destination} : {c.CustomerId}");
                    if (de.Type == EventType.Created)
                    {
                        _dbContext.Customers.Add(c);
                        _dbContext.SaveChanges();
                        break;
                    }

                    if (de.Type == EventType.Updated)
                    {
                        _dbContext.Customers.Update(c);
                        _dbContext.SaveChangesAsync();
                        break;
                    }

                    if (de.Type == EventType.Deleted)
                    {
                        _dbContext.Customers.Remove(c);
                        _dbContext.SaveChanges();
                    }

                    break;
                }
            }

        return Task.CompletedTask;
    }
}