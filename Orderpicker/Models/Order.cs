namespace Orderpicker.Models;

public class Order
{
    public int OrderId { get; set; }
    public OrderStatus OrderStatus { get; set; }
    
    public List<Product> Products { get; set; }
}