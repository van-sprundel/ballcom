using System.ComponentModel.DataAnnotations;
using BallCore;

namespace SupplierManagement.Models;

public class Product : IDomainModel
{
    [Key] public int ProductId { get; set; }

    public string Name { get; set; }
    public int SupplierId { get; set; }
    public virtual Supplier Supplier { get; set; }
}