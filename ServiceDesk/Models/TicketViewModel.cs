using BallCore.Enums;

namespace ServiceDesk.Models;

public class TicketViewModel
{
    public int TicketId { get; set; }
    public string TicketText { get; set; }
    public TicketStatus Status { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
}