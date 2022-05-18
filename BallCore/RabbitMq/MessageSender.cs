using System.Text.Json;
using RabbitMQ.Client;

namespace BallCore.RabbitMq;

public class MessageSender : IMessageSender, IDisposable
{
    private readonly IConnection? _connection;
    private readonly IModel? _channel;

    public MessageSender(params string[] queues)
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
        

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        foreach (var queueName in queues)
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void Send(string channelName, object message)
    {
        var props = _channel!.CreateBasicProperties();
        props.ContentType = "application/json";
        
        _channel.BasicPublish(exchange: "", routingKey: channelName, basicProperties: props, body: JsonSerializer.SerializeToUtf8Bytes(message));
    }
    
    public void Dispose()
    {
        Console.WriteLine("Stopping RabbitMq service");
        _channel?.Dispose();
        _connection?.Dispose();
    }
}