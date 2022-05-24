namespace ServiceDesk.Models;

public class TicketCreateForm
{
    public string TicketText { get; set; }
    public Status  Status { get; set; }
    
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
}