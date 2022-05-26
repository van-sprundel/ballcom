using BallCore.Enums;

namespace NotificationService.Models;

public class Ticket
{
    public int TicketId { get; set; }
    public TicketStatus  Status { get; set; }
    public int CustomerId { get; set; }
}