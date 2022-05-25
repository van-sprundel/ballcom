using BallCore;

namespace ServiceDesk.Models;

public class Ticket : IDomainModel
{
    public int TicketId { get; set; }
    public string TicketText { get; set; }
    public Status  Status { get; set; }
    
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    
    
}