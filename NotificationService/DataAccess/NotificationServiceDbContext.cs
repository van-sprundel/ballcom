using Microsoft.EntityFrameworkCore;
using NotificationService.Models;

namespace NotificationService.DataAccess;

public class NotificationServiceDbContext : DbContext
{
    public NotificationServiceDbContext(DbContextOptions<NotificationServiceDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
}