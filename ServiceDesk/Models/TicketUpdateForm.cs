using BallCore.Enums;

namespace ServiceDesk.Models;

public class TicketUpdateForm
{
    public int TicketId { get; set; }
    public TicketStatus  Status { get; set; }
}