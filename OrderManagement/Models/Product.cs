using BallCore;

namespace OrderManagement.Models;

public class Product : IDomainModel
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }

    public ICollection<OrderProduct> OrderProducts { get; set; }
}