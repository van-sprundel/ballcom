using System.Text.Json;
using RabbitMQ.Client;

namespace BallCore.RabbitMq;

public class MessageSender : IMessageSender, IDisposable
{
    private IConnection? _connection;
    private IModel? _channel;
    private readonly List<string> _queues;

    public MessageSender()
    {
        Console.WriteLine("Creating RabbitMq service");
        _queues = new List<string>();
        _queues.AddRange(new[]{ "general" });
        Start().Wait();
    }

    public Task Start()
    {
        Console.WriteLine("Starting RabbitMq service");

        return Task.Run(() =>
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "Rathalos",
                Password = "1234",
                Port = 5672,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            foreach (var queueName in _queues)
            {
                _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            }
        });
    }

    public Task Stop()
    {
        Console.WriteLine("Stopping RabbitMq service");
        
        return Task.Run(() =>
        {
            _channel?.Dispose();
            _connection?.Dispose();
        });
    }

    public void Send(string channelName, object message)
    {
        _channel.BasicPublish(exchange: "", routingKey: channelName, basicProperties: null, body: JsonSerializer.SerializeToUtf8Bytes(message));
    }
    
    public void Dispose()
    {
        Stop().Wait();
    }
}