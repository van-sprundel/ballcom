using Microsoft.EntityFrameworkCore;
using SupplierManagement.Models;

namespace SupplierManagement.DataAccess;

public class SupplierManagementDbContext : DbContext
{

    public SupplierManagementDbContext(DbContextOptions<SupplierManagementDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Product> Products { get; set; }
}