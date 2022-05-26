using InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.DataAccess;

public class InventoryManagementDbContext : DbContext
{
    public InventoryManagementDbContext(DbContextOptions<InventoryManagementDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Product> Products { get; set; }
}