using BallCore.Events;
using BallCore.RabbitMq;
using PaymentService.DataAccess;
using PaymentService.Models;
using RabbitMQ.Client;

namespace PaymentService;

public class PaymentMessageReceiver : MessageReceiver
{
    private readonly PaymentServiceDbContext _paymentServiceDbContext;
    private readonly IMessageSender _messageSender;

    public PaymentMessageReceiver(IConnection connection,
        PaymentServiceDbContext paymentServiceDbContext, IMessageSender messageSender) : base(connection,
        new[] { "payment" })
    {
        _paymentServiceDbContext = paymentServiceDbContext;
        _messageSender = messageSender;
    }

    // Example of how to handle message
    protected override Task HandleMessage(IEvent e)
    {
        Console.WriteLine("Received message");
        switch (e)
        {
            case DomainEvent { Payload: Customer customer } de:
            {
                switch (de.Type)
                {
                    case EventType.Created:
                    {
                        _paymentServiceDbContext.Customers.Add(customer);
                        break;
                    }
                    case EventType.Updated:
                    {
                        _paymentServiceDbContext.Customers.Update(customer);
                        _paymentServiceDbContext.SaveChanges();
                        break;
                    }
                    case EventType.Deleted:

                        _paymentServiceDbContext.Customers.Remove(customer);
                        _paymentServiceDbContext.SaveChanges();
                        break;
                }

                break;
            }
            case DomainEvent { Payload: Order order } de:
            {
                Console.WriteLine($"Received {de.Type} message {de.Name} from {de.Destination} : {order.IsPaid}");
                switch (de.Type)
                {
                    case EventType.Created:
                    {
                        _paymentServiceDbContext.Orders.Add(order);
                        break;
                    }
                    case EventType.Updated:
                    {
                        _paymentServiceDbContext.Orders.Update(order);
                        _paymentServiceDbContext.SaveChanges();
                        break;
                    }
                    case EventType.Deleted:
                    {
                        _paymentServiceDbContext.Orders.Remove(order);
                        _paymentServiceDbContext.SaveChanges();
                        break;
                    }
                }
                // if (de.Destination == "orderpicker_exchange" &&
                //     de.Type == EventType.Created &&
                //     !order.IsPaid)
                // {
                //     //send event back with link to pay for order
                //     //we can just make this /api/order/{id}/pay
                //     var path = "/" + order.OrderId + "/pay";
                //     _messageSender.Send(new RawEvent(
                //         JsonSerializer.SerializeToUtf8Bytes("http://paymentlink"+path),
                //         "paymentLink",
                //         "orderpicker"
                //     ));
                // }
                
                if (de.Destination == "transportmanagement_exchange" &&
                    de.Type == EventType.Updated &&
                    order.StatusProcess == StatusProcess.Arrived &&
                    !order.IsPaid)
                {
                    //send notification to notificationservice so customer gets an email
                    _messageSender.Send(new DomainEvent(order, EventType.Updated, "notificationservice"));
                }
                break;
            }
        }

        _paymentServiceDbContext.SaveChanges();
        return Task.CompletedTask;
    }
}