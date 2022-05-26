namespace SupplierManagement.Models;

using BallCore;
using System.ComponentModel.DataAnnotations;

public class Product : IDomainModel
{
    [Key]
    public int ProductId { get; set; }
    public string Name { get; set; }
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; }
}