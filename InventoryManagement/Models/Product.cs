using System.ComponentModel.DataAnnotations;
using BallCore;

namespace InventoryManagement.Models;

public class Product : IDomainModel
{
    [Key] public int ProductId { get; set; }

    public string Name { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
}