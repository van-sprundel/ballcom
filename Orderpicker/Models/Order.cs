using BallCore.Enums;

namespace Orderpicker.Models;

public class Order
{
    public int Id { get; set; }
    public StatusProcess Status { get; set; }
    public ICollection<OrderProduct> Products { get; set; }
}