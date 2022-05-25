using BallCore.Enums;

namespace TransportManagement.Models;

public class Order
{
    public int OrderId { get; set; }
    public string ArrivalCity { get; set; }
    public string ArrivalAdress { get; set; }
    public StatusProcess StatusProcess { get; set; }

    public int? TransportCompanyId { get; set; }
    public TransportCompany? TransportCompany { get; set; }
}