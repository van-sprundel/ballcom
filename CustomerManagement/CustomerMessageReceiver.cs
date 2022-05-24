using BallCore.Events;
using BallCore.RabbitMq;
using CustomerManagement.Models;
using OrderManagement.Models;

namespace CustomerManagement;

public class CustomerMessageReceiver : MessageReceiver
{
    public CustomerMessageReceiver() : base(new[] {"customer"},"customer_exchange")
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
                Console.WriteLine($"Received {de.Type} message ({de.Name}) from {de.Exchange}{de} : {c.FirstName}");
                break;
            }
            default:
            {
                Console.WriteLine("lol");
                break;
            }
        }
        
        return Task.CompletedTask;
    }
}