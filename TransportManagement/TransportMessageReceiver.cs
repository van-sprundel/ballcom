using BallCore.Events;
using BallCore.RabbitMq;
using RabbitMQ.Client;
using TransportManagement.DataAccess;
using TransportManagement.Models;

namespace TransportManagement;

public class TransportMessageReceiver : MessageReceiver
{
    private readonly TransportManagementDbContext _dbContext;

    public TransportMessageReceiver(IConnection connection, TransportManagementDbContext dbContext) :
        base(connection, new[] { "transport_management" })
    {
        _dbContext = dbContext;
    }

    protected override Task HandleMessage(IEvent e)
    {
        Console.WriteLine("Received message");
        if (e is DomainEvent de)
            switch (de.Payload)
            {
                case Order c:
                {
                    Console.WriteLine(
                        $"Received ex: {de.UseExchange} {de.Type} message ({de.Name}) from {de.Destination} : {c.ArrivalAdress}");
                    if (de.Type == EventType.Created)
                    {
                        _dbContext.Orders.Add(c);
                        _dbContext.SaveChanges();
                        break;
                    }

                    if (de.Type == EventType.Deleted)
                    {
                        _dbContext.Orders.Remove(c);
                        _dbContext.SaveChanges();
                        break;
                    }

                    if (de.Type == EventType.Updated)
                    {
                        _dbContext.Orders.Update(c);
                        _dbContext.SaveChanges();
                    }

                    break;
                }
            }

        return Task.CompletedTask;
    }
}