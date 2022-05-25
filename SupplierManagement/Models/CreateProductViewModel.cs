using System;
namespace SupplierManagement.Models;

public class CreateProductViewModel
{
    public string Name { get; set; }
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; }
}