using BallCore;

namespace OrderManagement.Models;

public class Customer : IDomainModel
{
    public int CustomerId { get; set; }
    public string Email { get; set; }
    public ICollection<Order> Orders { get; set; }
}