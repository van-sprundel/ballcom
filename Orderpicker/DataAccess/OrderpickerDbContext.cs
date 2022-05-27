using Microsoft.EntityFrameworkCore;
using Orderpicker.Models;

namespace Orderpicker.DataAccess;

public class OrderpickerDbContext : DbContext
{
    public OrderpickerDbContext(DbContextOptions<OrderpickerDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }
    public DbSet<Product> Products { get; set; }
}