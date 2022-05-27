using BallCore;

namespace Orderpicker.Models;

public class Product : IDomainModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
}