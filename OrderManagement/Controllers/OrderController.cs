using BallCore.Enums;
using BallCore.Events;
using BallCore.RabbitMq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagement.DataAccess;
using OrderManagement.Models;
using OrderManagement.ViewModels;

namespace OrderManagement.Controllers;

[Route("/api/[controller]")]
public class OrderController : Controller
{
    private readonly OrderManagementDbContext _dbContext;
    private readonly IMessageSender _rmq;

    public OrderController(OrderManagementDbContext dbContext, IMessageSender rmq)
    {
        _dbContext = dbContext;
        _rmq = rmq;
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

        order.OrderProducts = _dbContext.OrderProduct.Where(op => op.OrderId == orderId).ToList();
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
                    IsPaid = false,
                    OrderProducts = new List<OrderProduct>()
                };
                // insert order
                _dbContext.Orders.Add(order);
                await _dbContext.SaveChangesAsync();

                //PURE TESTING
                // Order order2 = new Order
                // {
                //     CustomerId = form.CustomerId,
                //     ArrivalCity = "TESTING",
                //     ArrivalAdress = "TESTING",
                //     OrderDate = DateTime.Now,
                //     StatusProcess = StatusProcess.Pending,
                //     Price = 900,
                //     IsPaid = false,
                //     OrderProducts = new List<OrderProduct>()
                // };
                //
                // _rmq.Send(new DomainEvent(order2, EventType.Created, "order", false));

                // return result
                return CreatedAtRoute("GetByOrderId", new { orderId = order.OrderId }, order);
            }

            return BadRequest();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Unable to save changes. ");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    [Route("{orderNumber}/{productNumber}", Name = "AddProductToOrder")]
    public async Task<IActionResult> AddProductToOrderAsync(int orderNumber, int productNumber)
    {
        try
        {
            int amountProducts =  _dbContext.OrderProduct.Count(op => op.OrderId == orderNumber);
            if (amountProducts < 20)
            {
                var order =  await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderNumber);
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productNumber);
                if (product != null && order != null)
                {
                    if (order.StatusProcess != StatusProcess.Pending)
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
                    order.Price += product.Price;
                    // Insert
                    _dbContext.OrderProduct.Add(orderProduct);
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

    [HttpPut]
    [Route("submit/{orderNumber}", Name = "SubmitOrder")]
    public async Task<IActionResult> SubmitOrderAsync(int orderNumber)
    {
        try
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderNumber);
            if (order == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "Order was not found.");
            }
            if (order.StatusProcess != StatusProcess.Pending)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Order has already been submitted. No changes allowed.");
            }
            
            int amountProducts = _dbContext.OrderProduct.Count(op => op.OrderId == orderNumber);
            if (amountProducts < 1)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Order needs at least one product");
            }

            order.StatusProcess = StatusProcess.Collecting;
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();
            
            // Send event
            _rmq.Send(new DomainEvent(order, EventType.Created, "order", false));

            return StatusCode(StatusCodes.Status200OK, order);
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Unable to save changes.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete]
    [Route("{orderNumber}/{productNumber}", Name = "RemoveProductFromOrder")]
    public async Task<IActionResult> RemoveProductFromOrderAsync(int orderNumber, int productNumber)
    {
        try
        {
            var orderProduct = await _dbContext.OrderProduct.FirstOrDefaultAsync(op =>
                    op.OrderId == orderNumber && op.ProductId == productNumber);
            var order =  await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderNumber);
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productNumber);
            // Remove
            if (orderProduct != null && order != null && product != null)
            {
                if (order.StatusProcess != StatusProcess.Pending)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "Order has already been submitted. No changes allowed.");
                }
                order.Price -= product.Price;
                _dbContext.OrderProduct.Remove(orderProduct);
                _dbContext.Orders.Update(order);
                await _dbContext.SaveChangesAsync();

                // return result
                return StatusCode(StatusCodes.Status204NoContent);
            }
            else
            {
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