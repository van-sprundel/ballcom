using Microsoft.AspNetCore.Mvc;
using PaymentService.DataAccess;

namespace PaymentService.Controllers;

public class PaymentController : Controller
{
    private readonly PaymentServiceDbContext _dbContext;

    public PaymentController(PaymentServiceDbContext dbContext)
    {
        _dbContext = dbContext;
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
            return await Task.FromResult(new NotFoundResult());
        }
        return await Task.FromResult(Ok(order));
    }
}