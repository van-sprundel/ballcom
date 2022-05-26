using BallCore;
using CustomerManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.DataAccess;

public class CustomerManagementDbContext : DbContext
{
    public CustomerManagementDbContext(DbContextOptions<CustomerManagementDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Customer>().HasKey(m => m.CustomerId);
        builder.Entity<Customer>().ToTable("Customer");
        base.OnModelCreating(builder);
    }

    public DbSet<Customer> Customers { get; set; }

}