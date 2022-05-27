using Microsoft.EntityFrameworkCore;

namespace EventService.Data;

public class EventContext : DbContext
{
    public EventContext(DbContextOptions<EventContext> options) : base(options)
    {
    }

    public DbSet<StoredDomainEvent> Events { get; set; }
}