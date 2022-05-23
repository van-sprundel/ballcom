using Microsoft.EntityFrameworkCore;
using OrderManagement.Models;

namespace OrderManagement.DataAccess;

public class OrderManagementDbContext : DbContext
{
    public OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Order> Orders { get; set; }
    //public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
}