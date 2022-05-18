namespace BallCore.RabbitMq;

public interface IMessageSender
{
    /// <summary>
    /// Send message to RabbitMQ broker
    /// </summary>
    /// <param name="channelName">The channel/queue name</param>
    /// <param name="message">The message to send (will be serialized to JSON)</param>
    public void Send(string channelName, object message);
}