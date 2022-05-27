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
                if (!_dbContext.Customers.Any(c => c.CustomerId == form.CustomerId))
                {
                    return StatusCode(StatusCodes.Status404NotFound, "Customer does not exist");
                }

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

                _rmq.Send(new DomainEvent(order, EventType.Created, "order_exchange_order", true));

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
    [Route("add-product", Name = "AddProductToOrder")]
    public async Task<IActionResult> AddProductToOrderAsync(AddProductToOrderForm form)
    {
        try
        {
            int amountProducts = _dbContext.OrderProduct.Count(op => op.OrderId == form.OrderId);
            if (amountProducts < 20)
            {
                var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == form.OrderId);
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == form.ProductId);
                if (product != null && order != null)
                {
                    if (order.StatusProcess != StatusProcess.Pending)
                    {
                        return StatusCode(StatusCodes.Status403Forbidden,
                            "Order has already been submitted. No changes allowed.");
                    }

                    if (product.Quantity < 1)
                    {
                        return StatusCode(StatusCodes.Status410Gone, "Order is out of stock");
                    }

                    var orderProduct = new OrderProduct
                    {
                        OrderId = form.OrderId,
                        ProductId = form.ProductId,
                        IsPicked = false
                    };
                    order.Price += product.Price;
                    // Insert
                    _dbContext.OrderProduct.Add(orderProduct);
                    _dbContext.Orders.Update(order);
                    await _dbContext.SaveChangesAsync();

                    OrderProductViewModel orderProductView = new OrderProductViewModel()
                    {
                        OrderProductId = orderProduct.ProductId,
                        OrderId = orderProduct.OrderId,
                        ProductId = orderProduct.ProductId,
                        IsPicked = orderProduct.IsPicked
                    };

                    _rmq.Send(
                        new DomainEvent(orderProductView, EventType.Created, "order_exchange_order_product", true));

                    // return result
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                        "ProductNumber or OrderNumber could not be found.");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status412PreconditionFailed,
                    "This order already contains 20 products, more are not allowed.");
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
                return StatusCode(StatusCodes.Status403Forbidden,
                    "Order has already been submitted. No changes allowed.");
            }

            var orderProducts = _dbContext.OrderProduct.Where(op => op.OrderId == orderNumber);
            if (!orderProducts.Any())
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Order needs at least one product");
            }

            order.StatusProcess = StatusProcess.Collecting;
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();

            // Send event
            _rmq.Send(new DomainEvent(order, EventType.Updated, "order_exchange_order", true));

            return StatusCode(StatusCodes.Status200OK, order);
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Unable to save changes.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete]
    [Route("delete", Name = "RemoveProductFromOrder")]
    public async Task<IActionResult> RemoveProductFromOrderAsync(RemoveProductFromOrderForm form)
    {
        try
        {
            var orderProduct = await _dbContext.OrderProduct.FirstOrDefaultAsync(op =>
                op.OrderId == form.OrderId && op.ProductId == form.ProductId);
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == form.OrderId);
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == form.ProductId);
            // Remove
            if (orderProduct != null && order != null && product != null)
            {
                if (order.StatusProcess != StatusProcess.Pending)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        "Order has already been submitted. No changes allowed.");
                }

                order.Price -= product.Price;
                _dbContext.OrderProduct.Remove(orderProduct);
                _dbContext.Orders.Update(order);
                await _dbContext.SaveChangesAsync();

                OrderProductViewModel orderProductView = new OrderProductViewModel()
                {
                    OrderProductId = orderProduct.ProductId,
                    OrderId = orderProduct.OrderId,
                    ProductId = orderProduct.ProductId,
                    IsPicked = orderProduct.IsPicked
                };

                _rmq.Send(new DomainEvent(orderProductView, EventType.Deleted, "order_exchange_order_product", true));

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