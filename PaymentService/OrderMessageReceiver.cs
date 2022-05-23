using BallCore.Events;
using BallCore.RabbitMq;
using PaymentService.Models;

namespace PaymentService;

public class OrderMessageReceiver : MessageReceiver
{
    public OrderMessageReceiver() : base(new[] { "general", "testnet" })
    {
    }

    // Example of how to handle message
    protected override Task HandleMessage(IEvent e)
    {
        Console.WriteLine("Received message");
        switch (e)
        {
            //Pattern matching: if event is domain event with customer
            case DomainEvent {Payload: Order order} de:
            {
                Console.WriteLine($"Received {de.Type} message {order.OrderId} : {order.isPaid}");
                break;
            }
        }
        
        return Task.CompletedTask;
    }
}