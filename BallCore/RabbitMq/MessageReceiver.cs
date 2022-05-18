using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BallCore.RabbitMq;

public abstract class MessageReceiver : IHostedService
{
    private readonly string[] _queues;
    private IConnection? _connection;
    private IModel? _channel;
    private AsyncEventingBasicConsumer? _consumer;

    protected MessageReceiver(string[] queues)
    {
        _queues = queues;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            Console.WriteLine("Creating and starting RabbitMq receiver service");

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
            foreach (var queueName in _queues)
                _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            
            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.Received += async (_, ea) =>
            {
                var res = await HandleMessage(ea.RoutingKey, ea.Body.ToArray());
                if(res)
                    _channel.BasicAck(ea.DeliveryTag, false);
                
                await Task.Yield();
            };

            foreach (var queueName in _queues)
            {
                var consumerTag = _channel.BasicConsume(queueName, false, _consumer);
                Console.WriteLine($"Consumer #{consumerTag} for {queueName}");
            }

            
        }, cancellationToken);
    }

    protected abstract Task<bool> HandleMessage(string channelName, byte[] body);

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stopping RabbitMq receiver service");
        _channel?.Dispose();
        _connection?.Dispose();
        _consumer = null;
        return Task.CompletedTask;
    }
}