using System.Text.Json;

namespace BallCore.Events;

/// <summary>
/// A domain event
/// </summary>
public class DomainEvent : IEvent
{
    /// <summary>
    /// The event type (Created, Updated, Deleted etc)
    /// </summary>
    public EventType Type { get; }
    
    /// <summary>
    /// The payload object (like a Customer)
    /// </summary>
    public IDomainModel Payload { get; }

    /// <inheritdoc cref="IEvent.Channel"/>
    public string Channel { get; }
    
    /// <summary>
    /// The event name (like CustomerCreated) but seperated with dot to ease parsing (Customer.Created)
    /// </summary>
    public string Name => $"{Type.GetType().Name}.{Type}";

    /// <inheritdoc cref="IEvent.Serialize"/>
    public byte[] Serialize() => JsonSerializer.SerializeToUtf8Bytes(Payload);

    /// <summary>
    /// Create new DomainEvent
    /// </summary>
    /// <param name="payload">The payload model object</param>
    /// <param name="type">The event type</param>
    /// <param name="channel">The channel the event must be sent to/is received from</param>
    public DomainEvent(IDomainModel payload, EventType type, string channel)
    {
        Type = type;
        Channel = channel;
        Payload = payload;
    }
}