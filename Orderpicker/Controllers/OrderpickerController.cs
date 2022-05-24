using BallCore.Enums;
using BallCore.Events;
using BallCore.RabbitMq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orderpicker.DataAccess;
using Orderpicker.Models;

namespace Orderpicker.Controllers;

[Route("/api/[controller]")]
public class OrderpickerController : Controller
{
    private readonly OrderManagementDbContext _dbContext;

    public OrderpickerController(OrderpickerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _dbContext.Orders.ToListAsync());
    }

    [HttpGet]
    [Route("{orderId}", Name = "GetByOrderId")]
    public async Task<IActionResult> GetByOrderId(int orderId)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order == null)
        {
            return NotFound();
        }

        order.Products = _dbContext.Product.Where(o => o.OrderId == orderId).ToList();
        return Ok(order);
    }

    [HttpGet]
    [Route("{productId}", Name = "GetByProductId")]
    public async Task<IActionResult> GetByProductId(int productId)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
        if (product == null)
        {
            return NotFound();
        }

        product.Orders = _dbContext.Orders.Where(p => p.ProductId == productId).ToList();
        return Ok(product);
    }

}