using BallCore.Events;
using BallCore.RabbitMq;
using PaymentService.Models;
using RabbitMQ.Client;

namespace PaymentService;

public class PaymentMessageReceiver : MessageReceiver
{
    public PaymentMessageReceiver(IConnection connection) : base(connection, new[] {"payment"})
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
                Console.WriteLine($"Received {de.Type} message {de.Name} from {de.Destination} : {order.isPaid}");
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