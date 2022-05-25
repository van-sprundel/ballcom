namespace SupplierManagement.Models;

public class SupplierViewModel
{
    public int SupplierId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public List<Product> Products { get; set; }
}