using System.ComponentModel.DataAnnotations;
using BallCore;

namespace PaymentService.Models;

public class Customer : IDomainModel
{
    [Key] public int CustomerId { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}