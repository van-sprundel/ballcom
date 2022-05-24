﻿namespace OrderManagement.Models;

public class Customer
{
    public int CustomerId { get; set; }
    public string Email { get; set; }
    public ICollection<Order> Orders { get; set; }

}