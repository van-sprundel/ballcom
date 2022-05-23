using System.Text.Json.Serialization;

namespace BallCore.Events;

/// <summary>
/// Represents an event
/// </summary>
public interface IEvent
{
    /// <summary>
    /// The channel the event must be send to/is received from
    /// </summary>
    [JsonIgnore]
    public string Channel { get; }
    
    /// <summary>
    /// The event name
    /// </summary>
    [JsonIgnore]
    public string Name { get; }

    /// <summary>
    /// Serialize event to bytes
    /// </summary>
    /// <returns>The data to send</returns>
    public byte[] Serialize();
}