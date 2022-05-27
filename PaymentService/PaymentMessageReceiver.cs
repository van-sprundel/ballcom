using BallCore.Events;
using BallCore.RabbitMq;
using PaymentService.DataAccess;
using PaymentService.Models;
using RabbitMQ.Client;

namespace PaymentService;

public class PaymentMessageReceiver : MessageReceiver
{
    private readonly IMessageSender _messageSender;
    private readonly PaymentServiceDbContext _paymentServiceDbContext;

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
            case DomainEvent { Payload: Customer c } de:
            {
                if (de.Type == EventType.Created)
                {
                    var customer = new Customer
                    {
                        Email = c.Email,
                        CustomerId = c.CustomerId,
                        FirstName = c.FirstName,
                        LastName = c.LastName
                    };
                    _paymentServiceDbContext.Customers.Add(customer);
                }
                else if (de.Type == EventType.Updated)
                {
                    var customer =
                        _paymentServiceDbContext.Customers.FirstOrDefault(x => x.CustomerId == c.CustomerId);
                    if (customer == null) break;

                    _paymentServiceDbContext.Customers.Update(customer);
                    _paymentServiceDbContext.SaveChanges();
                }
                else if (de.Type == EventType.Deleted)
                {
                    var customer = _paymentServiceDbContext.Customers.FirstOrDefault(x => x.CustomerId == c.CustomerId);
                    if (customer == null) break;

                    _paymentServiceDbContext.Customers.Remove(customer);
                    _paymentServiceDbContext.SaveChanges();
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
                        var o = new Order()
                        {
                            CustomerId = order.CustomerId,
                            IsPaid = order.IsPaid,
                            OrderId = order.OrderId,
                            StatusProcess = order.StatusProcess
                        };
                        _paymentServiceDbContext.Orders.Add(o);
                        break;
                    }
                    case EventType.Updated:
                    {
                        _paymentServiceDbContext.Orders.Update(order);
                        break;
                    }
                    case EventType.Deleted:
                    {
                        _paymentServiceDbContext.Orders.Remove(order);
                        break;
                    }
                }

                _paymentServiceDbContext.SaveChanges();
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
                    //send notification to notificationservice so customer gets an email
                    _messageSender.Send(new DomainEvent(order, EventType.Updated, "notificationservice"));

                break;
            }
        }

        _paymentServiceDbContext.SaveChanges();
        return Task.CompletedTask;
    }
}