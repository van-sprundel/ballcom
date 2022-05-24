using System.Text.Json.Serialization;

namespace BallCore.Events;

/// <summary>
/// Represents an event
/// </summary>
public interface IEvent
{
    /// <summary>
    /// The exchange the event must be send to
    /// </summary>
    [JsonIgnore]
    public string Destination { get; }
    
    /// <summary>
    /// Whether to use exchange or queue
    /// </summary>
    [JsonIgnore]
    public bool UseExchange { get; }

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