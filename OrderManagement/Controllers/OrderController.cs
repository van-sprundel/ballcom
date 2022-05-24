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

                //TODO: send event???

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
            int amountProducts = _dbContext.OrderProduct
                .Select(op => op.OrderId == orderNumber && op.ProductId == productNumber).Count();
            if (amountProducts < 20)
            {
                OrderProduct orderProduct = new OrderProduct 
                { 
                    OrderId = orderNumber,
                    ProductId = productNumber
                };
                // Insert

                _dbContext.OrderProduct.Add(orderProduct);
                await _dbContext.SaveChangesAsync();
            
                // TODO: send event???
            
                // return result
                return Ok();
            }
            else
            {
                ModelState.AddModelError("", "This order already contains 20 products, more are not allowed.");
                return StatusCode(StatusCodes.Status412PreconditionFailed, "This order already contains 20 products, more are not allowed.");

            }

        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Unable to save changes.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete]
    [Route("{orderNumber}/{productNumber}", Name = "AddProductToOrder")]
    public async Task<IActionResult> RemoveProductFromOrderAsync(int orderNumber, int productNumber)
    {
        try
        {
            var orderProduct = _dbContext.OrderProduct.FirstOrDefault(op =>
                    op.OrderId == orderNumber && op.ProductId == productNumber);
            // Remove
            if (orderProduct != null)
            {
                _dbContext.OrderProduct.Remove(orderProduct);
                await _dbContext.SaveChangesAsync();

                // TODO: send event???
            
                // return result
                return StatusCode(StatusCodes.Status204NoContent);
            }
            else
            {
                ModelState.AddModelError("", "Item does not exist.");
                return StatusCode(StatusCodes.Status404NotFound, "Item does not exist.");
            }
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Unable to save changes.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    } 

}