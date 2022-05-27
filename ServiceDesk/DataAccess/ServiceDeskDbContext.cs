using Microsoft.EntityFrameworkCore;
using ServiceDesk.Models;

namespace ServiceDesk.DataAccess;

public class ServiceDeskDbContext : DbContext
{
    public ServiceDeskDbContext(DbContextOptions<ServiceDeskDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
}