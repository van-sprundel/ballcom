using BallCore.Events;
using BallCore.RabbitMq;
using Microsoft.AspNetCore.Mvc;
using PaymentService.DataAccess;
using PaymentService.Models;

namespace PaymentService.Controllers;

[Route("/api/[controller]")]
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
    [Route("{customerId}/order/{orderId}/pay")]
    public async Task<IActionResult> PayOrder(int customerId, int orderId)
    {
        var order = await _dbContext.Orders.FindAsync(orderId);

        if (order == null) return await Task.FromResult(new NotFoundObjectResult("Couldn't find order"));

        if (order.IsPaid) return await Task.FromResult(new StatusCodeResult(304));

        order.IsPaid = true;
        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync();

        var invoice = new Invoice()
        {
            CustomerId = customerId,
            OrderId = orderId
        };

        await _dbContext.Invoices.AddAsync(invoice);
        await _dbContext.SaveChangesAsync();

        _rmq.Send(new DomainEvent(order, EventType.Updated, "order_paid_exchange", true));

        return await Task.FromResult(new OkObjectResult(order));
    }

    [HttpGet]
    [Route("{customerId}/invoices")]
    public async Task<IActionResult> GetInvoices(int customerId)
    {
        var customer = await _dbContext.Customers.FindAsync(customerId);

        if (customer == null) return await Task.FromResult(new NotFoundObjectResult("Couldn't find order"));
        
        

        return await Task.FromResult(Ok("test"));
    }
}