namespace OrderManagement.Models;

public class Order
{
    public int OrderId { get; set; }
    public string ArrivalCity { get; set; }
    public string ArrivalAdress { get; set; }
    public DateTime OrderDate { get; set; }
    public StatusProcess StatusProcess { get; set; }
    public double Price { get; set; }
    public bool isPaid { get; set; }

    public List<Product> Products;
    
    public int customerId { get; set; }
    public Customer Customer { get; set; }
}