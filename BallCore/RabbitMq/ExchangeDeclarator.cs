using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace BallCore.RabbitMq;

public class ExchangeDeclarator : IHostedService
{
    private readonly IModel? _channel;
    private readonly Dictionary<string, IEnumerable<string>> _exchanges;

    /// <summary>
    ///     Declare exchanges on application startup
    /// </summary>
    /// <param name="connection">The connection</param>
    /// <param name="exchanges">Dictionary with key (exchange name) and values (queue names to bind to exchange)</param>
    public ExchangeDeclarator(IConnection connection, Dictionary<string, IEnumerable<string>> exchanges)
    {
        _exchanges = exchanges;

        //Create channel within connection. Note: a connection can contain multiple channels, but we use a connection per message receiver instance
        _channel = connection.CreateModel();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            Console.WriteLine("Registering exchanges");
            foreach (var (name, queues) in _exchanges)
            {
                _channel.ExchangeDeclare(name, "fanout", true, false);
                foreach (var q in queues)
                {
                    _channel!.QueueDeclare(q, true, false, false,
                        null);
                    _channel.QueueBind(q, name, "");
                }
            }
        }, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Dispose();
        return Task.CompletedTask;
    }
}