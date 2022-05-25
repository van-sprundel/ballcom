using System.Text.Json;
using BallCore.Events;
using BallCore.RabbitMq;
using PaymentService.Models;
using RabbitMQ.Client;

namespace PaymentService;

public class PaymentMessageReceiver : MessageReceiver
{
    private readonly IMessageSender _messageSender;

    public PaymentMessageReceiver(IConnection connection, IMessageSender messageSender) : base(connection,
        new[] { "payment" })
    {
        _messageSender = messageSender;
    }

    // Example of how to handle message
    protected override Task HandleMessage(IEvent e)
    {
        Console.WriteLine("Received message");
        switch (e)
        {
            case DomainEvent { Payload: Order order } de:
            {
                Console.WriteLine($"Received {de.Type} message {de.Name} from {de.Destination} : {order.IsPaid}");
                if (de.Destination == "orderpicker_exchange" &&
                    de.Type == EventType.Created &&
                    !order.IsPaid)
                {
                    //TODO send event back with link to pay for order
                    //we can just make this /api/order/{id}/pay
                    var path = "/" + order.OrderId + "/pay";
                    _messageSender.Send(new RawEvent(
                        JsonSerializer.SerializeToUtf8Bytes("http://paymentlink"+path),
                        "paymentLink",
                        "orderpicker"
                    ));
                }

                if (de.Destination == "transportmanagement_exchange" &&
                    de.Type == EventType.Updated &&
                    order.StatusProcess == StatusProcess.Arrived &&
                    !order.IsPaid)
                {
                    //TODO send notification back with link to pay for order
                    //we can just make this /api/order/{id}/pay
                    // var notification = ;
                    // _messageSender.Send(new DomainEvent(notification, EventType.Updated, "notificationservice"));
                }

                break;
            }
        }

        return Task.CompletedTask;
    }
}