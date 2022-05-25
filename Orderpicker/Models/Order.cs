
using BallCore.Enums;

namespace Orderpicker.Models;

public class Order
{
    public int OrderId { get; set; }
    public StatusProcess OrderStatus { get; set; }
    public bool IsDone { get; set; }
    public ICollection<OrderProduct> OrderProducts { get; set; }

}