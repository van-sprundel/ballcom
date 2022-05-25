using BallCore.Enums;
namespace Orderpicker.Models;

public class Order
{
    public int OrderId { get; set; }
    public StatusProcess OrderStatus { get; set; }

    public List<Product> Products { get; set; }
}