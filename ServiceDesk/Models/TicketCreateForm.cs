using BallCore.Enums;

namespace ServiceDesk.Models;

public class TicketCreateForm
{
    public string TicketText { get; set; }
    public int CustomerId { get; set; }
}