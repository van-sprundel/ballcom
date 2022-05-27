using BallCore;
using BallCore.Events;

namespace Orderpicker.Models;

public class OrderProduct : IDomainModel
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public bool isPicked { get; set; }
}