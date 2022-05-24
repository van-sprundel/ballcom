using System.ComponentModel.DataAnnotations;

namespace PaymentService.Models;

public class Customer
{
    [Key]
    public int CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}