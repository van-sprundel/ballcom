using BallCore.Events;
using BallCore.RabbitMq;
using Orderpicker.DataAccess;
using Orderpicker.Models;
using RabbitMQ.Client;

namespace Orderpicker;

public class OrderPickerReceiver : MessageReceiver
{
    private readonly OrderpickerDbContext _dbContext;

    public OrderPickerReceiver(OrderpickerDbContext dbContext, IConnection connection) : base(connection, new[] { "orderpicker_client", "general"})
    {
        this._dbContext = dbContext;
    }

    // Example of how to handle message
    protected override Task HandleMessage(IEvent e)
    {
        Console.WriteLine("Received message");

        if (e is DomainEvent de)
        {
            switch (de.Payload)
            {
                case Product c:
                {
                    Console.WriteLine($"Received ex: {de.UseExchange} {de.Type} message ({de.Name}) from {de.Destination} : {c.Name}");
                        this._dbContext.Set<Product>().Add(c);
                    break;
                }
                case Order o:
                {
                    Console.WriteLine($"Received ex: {de.UseExchange} {de.Type} message ({de.Name}) from {de.Destination}");
                        this._dbContext.Set<Order>().Add(o);
                    break;
                }
            }
        }
        
        return Task.CompletedTask;
    }
}