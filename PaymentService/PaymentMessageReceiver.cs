using BallCore.Events;
using BallCore.RabbitMq;
using PaymentService.Models;

namespace PaymentService;

public class PaymentMessageReceiver : MessageReceiver
{
    public PaymentMessageReceiver() : base(new[] {"payment"},"payment_exchange")
    {
    }

    // Example of how to handle message
    protected override Task HandleMessage(IEvent e)
    {
        Console.WriteLine("Received message");
        switch (e)
        {
            case DomainEvent {Payload: Order order} de:
            {
                Console.WriteLine($"Received {de.Type} message {order.OrderId} : {order.isPaid}");
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