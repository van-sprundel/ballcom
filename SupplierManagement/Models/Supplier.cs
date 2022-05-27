using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SupplierManagement.Models;

public class Supplier
{
    [Key] public int SupplierId { get; set; }

    public string Name { get; set; }
    public string Email { get; set; }

    [JsonIgnore]
    public virtual List<Product> Products { get; set; }
}