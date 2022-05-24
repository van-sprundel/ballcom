namespace BallCore.Events;

public class RawEvent : IEvent
{
    public RawEvent(string channel, string exchange, string name, byte[] data)
    {
        Channel = channel;
        Exchange = exchange;
        Name = name;
        Data = data;
    }

    public string Channel { get; }
    public string Exchange { get; }
    public string Name { get; }
    public byte[] Data { get; }
    
    public byte[] Serialize()
    {
        return Data;
    }
}