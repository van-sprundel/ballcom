using BallCore.Events;
using BallCore.RabbitMq;
using CustomerManagement.DataAccess;
using CustomerManagement.Models;
using RabbitMQ.Client;

namespace CustomerManagement;

public class CustomerMessageReceiver : MessageReceiver
{
    private readonly CustomerManagementDbContext _dbContext;

    public CustomerMessageReceiver(CustomerManagementDbContext dbContext, IConnection connection) : base(connection, new[] {"customer", "general"})
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
                case Customer c:
                {
                    Console.WriteLine($"Received ex: {de.UseExchange} {de.Type} message ({de.Name}) from {de.Destination} : {c.FirstName}");
                    break;
                }
                case Order o:
                {
                    Console.WriteLine($"Received ex: {de.UseExchange} {de.Type} message ({de.Name}) from {de.Destination} : {o.IsPaid}");
                    break;
                }
            }
        }
        
        return Task.CompletedTask;
    }
}