using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orderpicker.DataAccess;
using Orderpicker.Models;
using BallCore.Enums;
using BallCore.Events;
using BallCore.RabbitMq;

namespace Orderpicker.Controllers;

[Route("/api/[controller]")]
public class OrderpickerController : Controller
{
    private readonly OrderpickerDbContext _dbContext;
    private readonly IMessageSender _messageSender;

    public OrderpickerController(OrderpickerDbContext dbContext, IMessageSender messageSender)
    {
        _dbContext = dbContext;
        _messageSender = messageSender;
    }

    [HttpGet]
    [Route("orders")]
    public async Task<IActionResult> GetOrdersAsync()
    {
        var orders = await _dbContext.Set<Order>().ToListAsync();

        return Ok(orders);
    }

    [HttpGet]
    [Route("{id}", Name = "GetByOrderId")]
    public async Task<IActionResult> GetOrderAsync(int id)
    {
        var order = await _dbContext.Set<Order>().FirstOrDefaultAsync(x => x.Id == id);
        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpGet]
    [Route("products")]
    public async Task<IActionResult> GetProductsAsync()
    {
        var products = await _dbContext.Set<Product>().ToListAsync();

        return Ok(products);
    }

    [HttpGet]
    [Route("{id}", Name = "GetByProductId")]
    public async Task<IActionResult> GetProductAsync(int id)
    {
        var product = await _dbContext.Set<Product>().FirstOrDefaultAsync(x => x.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    [Route("update")]
    public async Task<IActionResult> UpdateStatus(OrderUpdateform form)
    {
        var order = await this._dbContext
            .Set<Order>()
            .FirstOrDefaultAsync(x => x.Id == form.Id);

        if (order == null)
        {
            return this.NotFound();
        }

        order.Status = form.Status;

        this._dbContext
            .Set<Order>()
            .Update(order);

        // Send update to order management
        this._messageSender.Send(new DomainEvent(order, EventType.Updated, "order_management", false));

        return this.Ok();
    }

    [HttpPost]
    [Route("update-product")]
    public async Task<IActionResult> UpdateOrderProductStatus(OrderUpdateform form)
    {
        var orderProduct = await _dbContext.OrderProducts.Include(p => p.Order).FirstOrDefaultAsync(x => x.OrderProductId == form.Id);

        if (orderProduct == null)
        {
            return NotFound();
        }
        orderProduct.IsPicked = true;
        orderProduct.Product.Quantity -= 1;
        _dbContext.OrderProducts.Update(orderProduct);
        _dbContext.Products.Update(orderProduct.Product);

        await _dbContext.SaveChangesAsync();

        //TODO: Send event

        return Ok();
        
    }
}