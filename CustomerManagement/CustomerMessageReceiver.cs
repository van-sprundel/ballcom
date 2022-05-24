using BallCore.Events;
using BallCore.RabbitMq;
using CustomerManagement.Models;

namespace CustomerManagement;

public class CustomerMessageReceiver : MessageReceiver
{
    public CustomerMessageReceiver() : base(new[] {"customer"},new[]{ "customer_exchange" })
    {
    }

    // Example of how to handle message
    protected override Task HandleMessage(IEvent e)
    {
        Console.WriteLine("Received message");
        switch (e)
        {
            case DomainEvent {Payload: Customer c} de:
            {
                Console.WriteLine($"Received {de.Type} message ({de.Name}) from {de.Destination} : {c.FirstName}");
                break;
            }
            case DomainEvent {Payload: Order o} de:
            {
                Console.WriteLine($"Received {de.Type} message ({de.Name}) from {de.Destination} : {o.isPaid}");
                break;
            }
        }
        
        return Task.CompletedTask;
    }
}