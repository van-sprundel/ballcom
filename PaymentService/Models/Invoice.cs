using System.ComponentModel.DataAnnotations;

namespace PaymentService.Models;

public class Invoice
{
    [Key]
    public int InvoiceId { get; set; }
    
    public int InvoiceNumber { get; set; }

    public int CustomerId { get; set; }
    public virtual Customer Customer { get; set; }

    public int OrderId { get; set; }
    public virtual Order Order { get; set; }
}