using BallCore.Events;
using BallCore.RabbitMq;
using EventService.Data;
using RabbitMQ.Client;

namespace EventService;

/// <summary>
/// Receives events from *all* queues and stores them in the event-store, "Events" database with a simple relational schema
/// </summary>
public class EventsReceiver : MessageReceiver
{
    private readonly EventContext _context;

    private static readonly string[] Queues =
    {
        "notifications", "customer_management", "payment", "order_management", "inventory_management",
        "supplier_management", "transport_management", "orderpicker_client", "general", "servicedesk"
    };
    
    public EventsReceiver(IConnection connection, EventContext context) : base(connection, Queues)
    {
        _context = context;
    }

    protected override async Task HandleMessage(IEvent e)
    {
        if (e is DomainEvent de)
        {
            var storedEvent = StoredDomainEvent.Create(de);
            _context.Events.Add(storedEvent);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Stored DomainEvent '{e.Name}' from {e.Destination} as #{storedEvent.Id}");
        }
    }
}