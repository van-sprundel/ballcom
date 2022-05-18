using System.Text;
using BallCore.RabbitMq;

namespace CustomerManagement;

public class CustomerMessageReceiver : MessageReceiver
{
    protected override Task<bool> HandleMessage(string channelName, byte[] body)
    {
        Console.Error.WriteLine($"Received message from {channelName}:{Encoding.UTF8.GetString(body)}");
        return Task.FromResult(true);
    }

    public CustomerMessageReceiver() : base(new []{ "general", "testnet" }) { }
}