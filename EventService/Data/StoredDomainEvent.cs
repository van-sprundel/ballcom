using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using BallCore.Events;

namespace EventService.Data;

public class StoredDomainEvent
{
    [Key]
    public int Id { get; set; }

    public string Destination { get; set; }
    public bool IsExchange { get; set; }
    public string Name { get; set; }
    
    /// <summary>
    /// Json data
    /// </summary>
    public string Data { get; set; }
    
    public EventType Type { get; set; }

    public static StoredDomainEvent Create(DomainEvent e)
    {
        return new StoredDomainEvent
        {
            Destination = e.Destination,
            IsExchange = e.UseExchange,
            Name = e.Name,
            Type = e.Type,
            Data = JsonSerializer.Serialize(e.Payload, e.Payload.GetType())
        };
    }
}