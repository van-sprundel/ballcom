using System.Text.Json;

namespace BallCore.Events;

/// <summary>
///     A domain event
/// </summary>
public class DomainEvent : IEvent
{
    /// <summary>
    ///     Create new DomainEvent
    /// </summary>
    /// <param name="payload">The payload model object</param>
    /// <param name="type">The event type</param>
    /// <param name="destination">The destination the event must be received from</param>
    /// <param name="useExchange">whether the destination is an exchange or channel</param>
    public DomainEvent(IDomainModel payload, EventType type, string destination, bool useExchange = false)
    {
        Type = type;
        Destination = destination;
        UseExchange = useExchange;
        Payload = payload;
    }

    /// <summary>
    ///     The event type (Created, Updated, Deleted etc)
    /// </summary>
    public EventType Type { get; }

    /// <summary>
    ///     The payload object (like a Customer)
    /// </summary>
    public IDomainModel Payload { get; }

    /// <inheritdoc cref="IEvent.Destination" />
    public string Destination { get; }

    /// <inheritdoc cref="IEvent.UseExchange" />
    public bool UseExchange { get; }

    /// <summary>
    ///     The event name (like CustomerCreated) but seperated with dot to ease parsing (Customer.Created)
    /// </summary>
    public string Name => $"{Payload.GetType().Name}.{Type.ToString()}";

    /// <inheritdoc cref="IEvent.Serialize" />
    public byte[] Serialize()
    {
        return JsonSerializer.SerializeToUtf8Bytes(Payload, Payload.GetType());
    }
}