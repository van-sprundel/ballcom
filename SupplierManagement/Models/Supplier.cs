using System.ComponentModel.DataAnnotations;

namespace SupplierManagement.Models;

public class Supplier
{
    [Key] public int SupplierId { get; set; }

    public string Name { get; set; }
    public string Email { get; set; }

    public virtual List<Product> Products { get; set; }
}