using BallCore;

namespace NotificationService.Models;

public class Customer : IDomainModel
{
    public int CustomerId { get; set; }
    public string? Email { get; set; }
}