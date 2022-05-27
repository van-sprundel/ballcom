using System.ComponentModel.DataAnnotations;
using BallCore;

namespace NotificationService.Models;

public class Customer : IDomainModel
{
    [Key]
    public int CustomerId { get; set; }
    public string? Email { get; set; }
}