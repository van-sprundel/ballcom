using System.Reflection;
using System.Text.Json;
using BallCore.Events;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BallCore.RabbitMq;

/// <summary>
/// MessageReceiver
/// Inherit from this class and implement the HandleMessage method
/// </summary>
public abstract class MessageReceiver : IHostedService
{
    private readonly IConnection _connection;
    private IModel? _channel;
    private AsyncEventingBasicConsumer? _consumer;
    private readonly string[] _queues;

    /// <summary>
    /// Specify which queues you want to subscribe to
    /// </summary>
    /// <param name="connection">The RabbitMQ connection</param>
    /// <param name="queues">The queues to listen to</param>
    protected MessageReceiver(IConnection connection, IEnumerable<string> queues)
    {
        _queues = queues.ToArray();
        _connection = connection;
    }

    /// <summary>
    /// Start the receiver (automatically done by ASP when injected with AddHostedService)
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            Console.WriteLine("Creating and starting RabbitMq receiver service");
            
            //Create channel within connection. Note: a connection can contain multiple channels, but we use a connection per message receiver instance
            _channel = _connection.CreateModel();
            
            //Declare queues
            foreach (var q in _queues)
                _channel.QueueDeclare(queue: q, durable: true, exclusive: false, autoDelete: false, arguments: null);

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.Received += Consumer_Received;

            //Start consuming from queues
            foreach (var queue in _queues)
            {
                var consumerTag = _channel.BasicConsume(queue, true, _consumer);
                Console.WriteLine($"Consumer #{consumerTag} for {queue}");
            }
        }, cancellationToken);
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs ea)
    {
        //Check if event is domain event (contains .)
        if (ea.BasicProperties.Type.Contains("."))
        {
            // CustomerCreated => Customer.Created => payload = Customer, type = EventType.Created
            var typeOfPayload = ea.BasicProperties.Type.Split('.')[0];
            var typeOfEvent = ea.BasicProperties.Type.Split('.')[1];

            //Find specified type with reflection => "Customer" => CustomerManagement.Customer or InventoryManagement.Customer etc
            var type = Assembly.GetEntryAssembly()!.GetTypes()
                .FirstOrDefault(x => x.IsClass &&
                                     x.IsAssignableTo(typeof(IDomainModel)) &&
                                     x.Name == typeOfPayload);
            if (type == null)
            {
                await Console.Error.WriteLineAsync($"Model type {typeOfPayload} not found");
                return;
            }

            if (Enum.TryParse(typeOfEvent, true, out EventType eventType))
            {
                //1. Deserialize body to object of expected type
                var obj = (IDomainModel)JsonSerializer.Deserialize(ea.Body.ToArray(), type)!;

                var isExchange = !string.IsNullOrEmpty(ea.Exchange);
                
                //2. Create Event object and call handler
                await HandleMessage(new DomainEvent(obj, eventType, isExchange ? ea.Exchange : ea.RoutingKey,
                    isExchange));
            }
            else
            {
                await Console.Error.WriteLineAsync($"Event type {typeOfEvent} does not exist");
                return;
            }
        }
        else
        {
            //Not a domain event
            var isExchange = !string.IsNullOrEmpty(ea.Exchange);
            await HandleMessage(new RawEvent(ea.Body.Span, ea.BasicProperties.Type, isExchange ? ea.Exchange : ea.RoutingKey, isExchange));
        }

        await Task.Yield();
    }

    /// <summary>
    /// Handle receive message
    /// </summary>
    /// <param name="e">The event that is received</param>
    /// <returns>True if ACK must be sent</returns>
    protected abstract Task HandleMessage(IEvent e);

    /// <summary>
    /// Stop the service and disconnect
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stopping RabbitMq receiver service");
        _channel?.Dispose();
        _consumer = null;
        return Task.CompletedTask;
    }
}