using BallCore.Events;
using RabbitMQ.Client;

namespace BallCore.RabbitMq;

/// <summary>
/// Default sender implementation
/// </summary>
public class MessageSender : IMessageSender, IDisposable
{
    private readonly IModel? _channel;
    private readonly List<string> _declaredQueues;

    public MessageSender(IConnection connection)
    {
        Console.WriteLine("Creating and starting RabbitMq service");

        _channel = connection.CreateModel();
        _declaredQueues = new List<string>();
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

        _channel.BasicQos(0, 1, false);

        if (!e.UseExchange && !_declaredQueues.Contains(e.Destination))
        {
            _channel.QueueDeclare(queue: e.Destination, durable: true, exclusive: false, autoDelete: false,
                arguments: null);
            _declaredQueues.Add(e.Destination);
        }

        //Publish message to queue
        _channel.BasicPublish(exchange: e.UseExchange ? e.Destination : "", e.UseExchange ? "" : e.Destination, basicProperties: props, body: e.Serialize(),
            mandatory: true);
    }

    public void Dispose()
    {
        Console.WriteLine("Stopping RabbitMq service");
        _channel?.Dispose();
    }
}