using BallCore.Events;
using BallCore.RabbitMq;
using InventoryManagement.DataAccess;
using InventoryManagement.Models;
using RabbitMQ.Client;

public class InventoryMessageReceiver : MessageReceiver
{
    private readonly InventoryManagementDbContext _dbContext;

    public InventoryMessageReceiver(InventoryManagementDbContext dbContext, IConnection connection) : base(connection, new[] { "inventory_management" })
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

                        if(de.Type == EventType.Created)
                        {
                            // Save product
                            _dbContext.Set<Product>().Add(c);
                        }
                        if (de.Type == EventType.Updated)
                        {
                            // Quantity minus 1;
                            c.Quantity -= 1;

                            // Save product
                            _dbContext.Set<Product>().Update(c);
                        }
                        
                        _dbContext.SaveChanges();
                        break;
                    }
            }
        }

        return Task.CompletedTask;
    }
}