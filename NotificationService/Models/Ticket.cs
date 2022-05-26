using BallCore;
using BallCore.Enums;

namespace NotificationService.Models;

public class Ticket : IDomainModel
{
    public int TicketId { get; set; }
    public TicketStatus  Status { get; set; }
    public int CustomerId { get; set; }
}