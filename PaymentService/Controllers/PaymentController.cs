using BallCore.Events;
using BallCore.RabbitMq;
using Microsoft.AspNetCore.Mvc;
using PaymentService.DataAccess;

namespace PaymentService.Controllers;

public class PaymentController : Controller
{
    private readonly PaymentServiceDbContext _dbContext;
    private readonly IMessageSender _rmq;

    public PaymentController(IMessageSender rmq, PaymentServiceDbContext dbContext)
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

    [HttpPost]
    [Route("api/order/{orderId}/pay")]
    public async Task<IActionResult> PayOrder(int orderId)
    {
        var order = await _dbContext.Orders.FindAsync(orderId);
        
        if (order == null)
        {
            return await Task.FromResult(new NotFoundObjectResult("Couldn't find order"));
        }

        if (order.IsPaid)
        {
            return await Task.FromResult(new StatusCodeResult(304));
        }

        order.IsPaid = true;
        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync();
        
        //TODO create invoice

        return await Task.FromResult(new OkObjectResult(order));
    }
}