using BallCore.Events;

namespace BallCore.RabbitMq;

public interface IMessageSender
{
    /// <summary>
    /// Send message to RabbitMQ broker
    /// </summary>
    /// <param name="e">The event to send</param>
    public void Send(IEvent e);
}