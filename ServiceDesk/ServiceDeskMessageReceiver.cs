using BallCore.Events;
using BallCore.RabbitMq;
using RabbitMQ.Client;
using ServiceDesk.DataAccess;
using ServiceDesk.Models;

namespace ServiceDesk;

public class ServiceDeskMessageReceiver : MessageReceiver
{
    private readonly ServiceDeskDbContext _dbContext;

    public ServiceDeskMessageReceiver(IConnection connection, ServiceDeskDbContext dbContext) : base(connection,
        new[] { "customer" })
    {
        _dbContext = dbContext;
    }

    protected override Task HandleMessage(IEvent e)
    {
        Console.WriteLine("Received message");
        if (e is DomainEvent de)
            switch (de.Payload)
            {
                case Customer c:
                {
                    Console.WriteLine(
                        $"Received ex: {de.UseExchange} {de.Type} message ({de.Name}) from {de.Destination} : {c.Email}");
                    if (de.Type == EventType.Updated)
                    {
                        // Update het order.
                        _dbContext.Customers.Add(c);
                        _dbContext.SaveChanges();
                    }

                    break;
                }
            }

        return Task.CompletedTask;
    }
}