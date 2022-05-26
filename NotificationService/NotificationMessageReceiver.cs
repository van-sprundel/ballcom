using BallCore.Events;
using BallCore.RabbitMq;
using NotificationService.Models;
using RabbitMQ.Client;

namespace NotificationService;

public class NotificationMessageReceiver : MessageReceiver
{

    public NotificationMessageReceiver(IConnection connection) :
        base(connection, new[] { "notifications" })
    {
        
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
                    if (de.Type == EventType.Updated)
                    {
                        
                        // Send Email if statusProcess has changed.
                        break;
                    }
                    break;
                }
            }
        }
        
        return Task.CompletedTask;
    }
}