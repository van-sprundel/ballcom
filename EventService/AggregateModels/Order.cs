using BallCore;

namespace EventService.AggregateModels;

public class Order : IDomainModel
{
    public int OrderId { get; set; }
    public double Price { get; set; }
    public bool? IsPaid { get; set; }
}