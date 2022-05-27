using System.ComponentModel.DataAnnotations;
using BallCore;

namespace CustomerManagement.Models;

public class Customer : IDomainModel
{
    [Key] public int CustomerId { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
}