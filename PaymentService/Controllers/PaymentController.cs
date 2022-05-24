using BallCore.Events;
using BallCore.RabbitMq;
using Microsoft.AspNetCore.Mvc;
using PaymentService.DataAccess;

namespace PaymentService.Controllers;

public class PaymentController : Controller
{
    private readonly PaymentServiceDbContext _dbContext;
    private readonly IMessageSender _rmq;

    public PaymentController(IMessageSender rmq,PaymentServiceDbContext dbContext)
    {
        _dbContext = dbContext;
        _rmq = rmq;
    }

    [HttpGet]
    [Route("test")]
    public async Task<IActionResult> Test()
    {
        return await Task.FromResult(Ok("test"));
    }

    [HttpGet]
    [Route("api/order/{orderId}")]
    public async Task<IActionResult> GetOrder(int orderId)
    {
        var order = await _dbContext.Orders.FindAsync(orderId);
        if (order == null)
        {
            return await Task.FromResult(new NotFoundObjectResult("Couldn't find order"));
        }
        return await Task.FromResult(Ok(order));
    }
    
    [HttpPost]
    [Route("api/order/{orderId}")]
    public async Task<IActionResult> PayOrder(int orderId)
    {
        var order = await _dbContext.Orders.FindAsync(orderId);
        if (order == null)
        {
            return await Task.FromResult(new NotFoundObjectResult("Couldn't find order"));
        }

        if (!order.isPaid)
        {
            order.isPaid = true;
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();

            //Send domain event to broker
            _rmq.Send(new DomainEvent(order, EventType.Updated, "payment_exchange",true));
        }
        
        return await Task.FromResult(Ok(order));
    }
}