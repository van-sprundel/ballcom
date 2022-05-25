namespace SupplierManagement.Models;
using System.ComponentModel.DataAnnotations;

public class Supplier
{
    [Key]
    public int SupplierId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public List<Product> Products { get; set; }
}