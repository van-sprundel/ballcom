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
        new[] { "servicedesk" })
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
                    if (de.Type == EventType.Created)
                    {
                        var customer = new Customer()
                        {
                            Email = c.Email,
                            CustomerId = c.CustomerId,
                            LastName = c.LastName
                        };
                        _dbContext.Customers.Add(customer);
                        _dbContext.SaveChanges();
                    }
                    if (de.Type == EventType.Updated)
                    {
                        var customer = new Customer()
                        {
                            Email = c.Email,
                            CustomerId = c.CustomerId,
                            LastName = c.LastName
                        };
                        _dbContext.Customers.Update(customer);
                        _dbContext.SaveChanges();
                    }

                    break;
                }
            }

        return Task.CompletedTask;
    }
}