using BallCore;
using BallCore.Enums;

namespace NotificationService.Models;

public class Order : IDomainModel
{
    public int OrderId { get; set; }
    public StatusProcess? StatusProcess { get; set; }
    public bool? IsPaid { get; set; }
    public int CustomerId { get; set; }
}