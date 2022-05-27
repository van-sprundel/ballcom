using BallCore.Events;
using BallCore.RabbitMq;
using InventoryManagement.DataAccess;
using InventoryManagement.Models;
using RabbitMQ.Client;

public class InventoryMessageReceiver : MessageReceiver
{
    private readonly InventoryManagementDbContext _dbContext;

    public InventoryMessageReceiver(InventoryManagementDbContext dbContext, IConnection connection) : base(connection,
        new[] { "inventory_management" })
    {
        _dbContext = dbContext;
    }

    // Example of how to handle message
    protected override Task HandleMessage(IEvent e)
    {
        Console.WriteLine("Received message");

        if (e is DomainEvent de)
            switch (de.Payload)
            {
                case Product c:
                {
                    Console.WriteLine(
                        $"Received ex: {de.UseExchange} {de.Type} message ({de.Name}) from {de.Destination} : {c.Name}");

                    // Save product
                    _dbContext.Set<Product>().Add(c);
                    break;
                }
            }

        return Task.CompletedTask;
    }
}