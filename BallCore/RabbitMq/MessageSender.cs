using BallCore.Events;
using RabbitMQ.Client;

namespace BallCore.RabbitMq;

/// <summary>
/// Default sender implementation
/// </summary>
public class MessageSender : IMessageSender, IDisposable
{
    private readonly IConnection? _connection;
    private readonly IModel? _channel;
    private readonly List<string> _queues;

    public MessageSender()
    {
        Console.WriteLine("Creating and starting RabbitMq service");

        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq",
            Port = 5672,
            UserName = "Rathalos",
            Password = "1234",
            DispatchConsumersAsync = true
        };
        
        //Create connection with broker
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _queues = new List<string>();
    }

    /// <summary>
    /// Send message to RabbitMQ broker
    /// </summary>
    /// <param name="e">The event to send</param>
    public void Send(IEvent e)
    {
        var props = _channel!.CreateBasicProperties();
        props.ContentType = "application/json";
        props.Type = e.Name;
        
        //Create queue if it does not exist
        if (!_queues.Contains(e.Channel))
        {
            _channel.QueueDeclare(queue: e.Channel, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _queues.Add(e.Channel);
        }
        
        //Publish message to queue
        _channel.BasicPublish(exchange: "", routingKey: e.Channel, basicProperties: props, body: e.Serialize());
    }

    public void Dispose()
    {
        Console.WriteLine("Stopping RabbitMq service");
        _queues.Clear();
        _channel?.Dispose();
        _connection?.Dispose();
    }
}