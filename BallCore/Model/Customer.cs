namespace BallCore.Model;

public class Customer
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastNmae { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }

    public Customer()
    {
    }
}