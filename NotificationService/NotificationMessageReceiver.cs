using BallCore.Enums;
using BallCore.Events;
using BallCore.RabbitMq;
using NotificationService.DataAccess;
using NotificationService.Models;
using RabbitMQ.Client;

namespace NotificationService;

public class NotificationMessageReceiver : MessageReceiver
{
    private readonly NotificationServiceDbContext _dbContext;

    public NotificationMessageReceiver(IConnection connection, NotificationServiceDbContext dbContext) :
        base(connection, new[] { "notifications" })
    {
        _dbContext = dbContext;
    }

    protected override Task HandleMessage(IEvent e)
    {
        Console.WriteLine("Received message");
        if (e is DomainEvent de)
        {
            switch (de.Payload)
            {
                case Order c:
                {
                    Console.WriteLine($"Received ex: {de.UseExchange} {de.Type} message ({de.Name}) from {de.Destination} : {c.CustomerId}");
                    if (de.Type == EventType.Created)
                    {
                        Console.WriteLine("Sending email about order being submitted");
                       //TODO:
                        // Send email order submitted
                        break;
                    }
                    
                    
                    if (de.Type == EventType.Updated)
                    {
                        Console.WriteLine("Sending Email about order updated");
                        //TODO: Send Email statusProcess has changed.
                        if (c.StatusProcess == StatusProcess.Arrived && c.IsPaid == false)
                        {
                            // TODO: Send reminder to pay.
                        }
                        break;
                    }
                    break;
                }
                case Ticket c:
                {
                    Console.WriteLine($"Received ex: {de.UseExchange} {de.Type} message ({de.Name}) from {de.Destination} : {c.CustomerId}");
                    if (de.Type == EventType.Created)
                    {
                        Console.WriteLine("Sending Email about ticket created");
                        //TODO: Send email
                        break;
                    }
                    if (de.Type == EventType.Updated)
                    {
                        Console.WriteLine("Sending Email about ticket created");
                        //TODO: Send email
                        break;
                    }
                    break;
                }
                case Customer c:
                {
                    Console.WriteLine($"Received ex: {de.UseExchange} {de.Type} message ({de.Name}) from {de.Destination} : {c.CustomerId}");
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
                        break;
                    }
                    break;
                    
                    
                }
                    
            }
        }
        
        return Task.CompletedTask;
    }
}