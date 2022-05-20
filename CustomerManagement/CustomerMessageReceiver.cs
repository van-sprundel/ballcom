using System.Text;
using System.Text.Json;
using BallCore.Model;
using BallCore.RabbitMq;

namespace CustomerManagement;

public class CustomerMessageReceiver : MessageReceiver
{
    protected override Task<bool> HandleMessage(string channelName, byte[] body)
    {
        var customer =JsonSerializer.Deserialize<Customer>(body)!;
        Console.Error.WriteLine($"Received message from {channelName} : {customer?.FirstName}");
        return Task.FromResult(true);
    }

    public CustomerMessageReceiver() : base(new[] { "general", "testnet" })
    {
    }
}