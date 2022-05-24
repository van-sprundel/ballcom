using System.Diagnostics;
using BallCore.Events;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace BallCore.RabbitMq;

/// <summary>
/// Default sender implementation
/// </summary>
public class MessageSender : IMessageSender, IDisposable
{
    private readonly IConnection? _connection;
    private readonly IModel? _channel;
    private readonly string _exchange;
    private readonly string[] _queues;

    public MessageSender(string[] queues, string exchange)
    {
        Console.WriteLine("Creating and starting RabbitMq service");

        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq",
            Port = 5672,
            UserName = "Rathalos",
            Password = "1234",
            // DispatchConsumersAsync = true
        };

        //Create connection with broker
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _exchange = exchange;
        _queues = queues;
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

        _channel.ExchangeDeclare(_exchange, "fanout", durable: true, autoDelete: false);
        _channel.BasicQos(0, 1, false);
        foreach (var queueName in _queues)
        {
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false,
                arguments: null);
            _channel.QueueBind(queueName, _exchange, "", null);
        }

        //Publish message to queue
        if (e.UseExchange)
        {
            _channel.BasicPublish(exchange: e.Destination, "", basicProperties: props, body: e.Serialize(),
                mandatory: true);
        }
        else
        {
            _channel.BasicPublish(exchange: "", e.Destination, basicProperties: props, body: e.Serialize(),
                mandatory: true);
        }
    }

    public void Dispose()
    {
        Console.WriteLine("Stopping RabbitMq service");
        _channel?.Dispose();
        _connection?.Dispose();
    }
}