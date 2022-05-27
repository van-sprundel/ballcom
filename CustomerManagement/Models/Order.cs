using BallCore;

namespace CustomerManagement.Models;

public class Order : IDomainModel
{
    public int OrderId { get; set; }
    public bool IsPaid { get; set; }
}