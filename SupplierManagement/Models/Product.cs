namespace SupplierManagement.Models;

public class Product
{
    public int ProductId { get; set; }
    public  string Name { get; set; }

    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; }
}