using Microsoft.EntityFrameworkCore;
using TransportManagement.Models;

namespace TransportManagement.DataAccess;

public class TransportManagementDbContext : DbContext
{
    public TransportManagementDbContext(DbContextOptions<TransportManagementDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<TransportCompany> TransportCompanies { get; set; }
    public DbSet<Order> Orders { get; set; }

}