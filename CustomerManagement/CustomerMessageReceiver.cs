using BallCore.Events;
using BallCore.RabbitMq;
using CustomerManagement.Models;

namespace CustomerManagement
{

public class CustomerMessageReceiver : MessageReceiver
{
    public CustomerMessageReceiver() : base(new[] { "general", "testnet" })
    {
    }

    // Example of how to handle message
    protected override Task HandleMessage(IEvent e)
    {
        Console.WriteLine("Received message");
        switch (e)
        {
            //Pattern matching: if event is domain event with customer
            case DomainEvent {Payload: Customer c} de:
            {
                Console.WriteLine($"Received {de.Type} message ({de.Name}) from {de.Channel}{de} : {c.FirstName}");
                break;
            }
        }
        
        return Task.CompletedTask;
    }
}