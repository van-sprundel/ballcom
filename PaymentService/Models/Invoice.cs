namespace PaymentService.Models;

public class Invoice
{
    public int InvoiceId { get; set; }
    public int InvoiceNumber { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; }

    public int Orderid { get; set; }
    public Order Order { get; set; }
}