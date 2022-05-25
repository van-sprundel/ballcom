using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orderpicker.DataAccess;
using Orderpicker.Models;
using BallCore.Enums;

namespace Orderpicker.Controllers;

[Route("/api/[controller]")]
public class OrderpickerController : Controller
{
    private readonly OrderpickerDbContext _dbContext;

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
        order.OrderProducts = _dbContext.OrderProducts.Where(op => op.OrderId == orderId).ToList();
        return Ok(order);
    }

    [HttpPost]
    [Route("{orderNumber}/{productNumber}", Name = "AddProductToOrder")]
    public async Task<IActionResult> AddProductToOrderAsync(int orderNumber, int productNumber)
    {
        try
        {
            int amountProducts = _dbContext.OrderProducts.Count(op => op.OrderId == orderNumber);
            if (amountProducts < 20)
            {
                var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderNumber);
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productNumber);
                if (product != null && order != null)
                {
                    if (order.OrderStatus != StatusProcess.Pending)
                    {
                        return StatusCode(StatusCodes.Status403Forbidden, "Order has already been submitted. No changes allowed.");
                    }
                    if (product.Quantity < 1)
                    {
                        return StatusCode(StatusCodes.Status410Gone, "Order is out of stock");
                    }
                    OrderProduct orderProduct = new OrderProduct
                    {
                        OrderId = orderNumber,
                        ProductId = productNumber
                    };
                    // Insert
                    _dbContext.OrderProducts.Add(orderProduct);
                    _dbContext.Orders.Update(order);
                    await _dbContext.SaveChangesAsync();

                    // return result
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound, "ProductNumber or OrderNumber could not be found.");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status412PreconditionFailed, "This order already contains 20 products, more are not allowed.");

            }

        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Unable to save changes.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

}