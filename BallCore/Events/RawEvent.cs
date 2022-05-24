namespace BallCore.Events;

public class RawEvent : IEvent
{
    /// <summary>
    /// Create new RawEvent
    /// </summary>
    /// <param name="payload">The binary payload</param>
    /// <param name="name">The command/event name</param>
    /// <param name="destination">The destination to send the data to (exchange or queue)</param>
    /// <param name="useExchange">Whether to use exchange or queue</param>
    public RawEvent(ReadOnlySpan<byte> payload, string name, string destination, bool useExchange = false)
    {
        Destination = destination;
        Name = name;
        Data = payload.ToArray();
        UseExchange = useExchange;
    }

    public string Destination { get; }
    public bool UseExchange { get; }
    public string Name { get; }
    public byte[] Data { get; }

    public byte[] Serialize()
    {
        return Data;
    }
}