namespace BallCore.Events;

public class RawEvent : IEvent
{
    public RawEvent(string name, byte[] data, string destination, bool useExchange)
    {
        Destination = destination;
        Name = name;
        Data = data;
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