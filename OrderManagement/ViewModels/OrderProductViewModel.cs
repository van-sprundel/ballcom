using BallCore;

namespace OrderManagement.ViewModels;

public class OrderProductViewModel : IDomainModel
{
    public int OrderProductId { get; set; }
    public int OrderId { get; set; }

    public int ProductId { get; set; }
}