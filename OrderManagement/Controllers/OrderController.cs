using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.EntityFrameworkCore;
using OrderManagement.DataAccess;
using OrderManagement.Models;
using OrderManagement.ViewModels;

namespace OrderManagement.Controllers;

[Route("/api/[controller]")]
public class OrderController : Controller
{
    OrderManagementDbContext _dbContext;

    public OrderController(OrderManagementDbContext dbContext)
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
        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> BeginOrderAsync([FromBody] OrderViewModel form)
    {
        try
        {
            if (ModelState.IsValid)
            {
                
                Order order = new Order
                {
                    CustomerId = form.CustomerId,
                    ArrivalCity = form.ArrivalCity,
                    ArrivalAdress = form.ArrivalAdress,
                    OrderDate = DateTime.Now,
                    StatusProcess = StatusProcess.Pending,
                    Price = 0,
                    IsPaid = false
                };
                // insert order
                _dbContext.Orders.Add(order);
                await _dbContext.SaveChangesAsync();

                //TODO: send event

                // return result
                return CreatedAtRoute("GetByOrderId", new { orderId = order.OrderId }, order);
            }

            return BadRequest();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Unable to save changes. ");
            return StatusCode(StatusCodes.Status500InternalServerError);
            throw;
        }
    }

    [HttpPost]
    [Route("{orderNumber}/{productNumber}", Name = "AddProductToOrder")]
    public async Task<IActionResult> AddProductToOrderAsync(int orderNumber, int productNumber)
    {
        try
        {
            OrderProduct orderProduct = new OrderProduct
            {
                OrderId = orderNumber,
                ProductId = productNumber
            };
            // Insert
            _dbContext.OrderProduct.Add(orderProduct);
            await _dbContext.SaveChangesAsync();
            
            // TODO: send event
            
            // return result
            return Ok();

        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Unable to save changes.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

}