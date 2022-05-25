using BallCore;

namespace ServiceDesk.Models;

public class Customer : IDomainModel
{
    public int CustomerId { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}