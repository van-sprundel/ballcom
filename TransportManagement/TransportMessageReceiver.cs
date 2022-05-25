using BallCore.Events;
using BallCore.RabbitMq;
using RabbitMQ.Client;
using TransportManagement.DataAccess;

namespace TransportManagement;

public class TransportMessageReceiver : MessageReceiver
{
    private readonly TransportManagementDbContext _dbContext;

    public TransportMessageReceiver(IConnection connection, TransportManagementDbContext dbContext) : base(connection,
        new[] { "order", "general" })
    {
        _dbContext = dbContext;
    }

    protected override Task HandleMessage(IEvent e)
    {
        
        
        return Task.CompletedTask;
    }

}