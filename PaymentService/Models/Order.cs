using System.ComponentModel.DataAnnotations;
using BallCore;

namespace PaymentService.Models;

public class Order : IDomainModel
{
    [Key]
    public int OrderId { get; set; }
    public bool IsPaid { get; set; }
    public StatusProcess StatusProcess { get; set; }


}