using BallCore.Enums;
using BallCore.Events;
using BallCore.RabbitMq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportManagement.DataAccess;
using TransportManagement.Models;

namespace TransportManagement.Controllers;

[Route("/api/[controller]")]
public class OrderController : Controller
{
    private readonly TransportManagementDbContext _dbContext;
    private readonly IMessageSender _rmq;

    public OrderController(TransportManagementDbContext dbContext, IMessageSender rmq)
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
        var order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId);
        if (order == null) return NotFound();

        return Ok(order);
    }

    [HttpPut]
    [Route("add-company", Name = "AddTransportCompanyToOrder")]
    public async Task<IActionResult> AddTransportCompanyToOrder(AddCompanyToOrderForm form)
    {
        try
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == form.OrderId);
            var transportCompany =
                await _dbContext.TransportCompanies.FirstOrDefaultAsync(t => t.TransportCompanyId == form.TransportId);
            if (order == null || transportCompany == null) return NotFound();

            if (order.StatusProcess == StatusProcess.Arrived)
                return StatusCode(StatusCodes.Status403Forbidden, "Order has already arrived.");

            order.TransportCompanyId = form.TransportId;
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status202Accepted);
        }
        catch (DbUpdateException)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut]
    [Route("{orderId}", Name = "ChangeStatusProcess")]
    public async Task<IActionResult> ChangeStatusFromOrder(int orderId)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest();

            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null) return NotFound();
            if (order.StatusProcess == StatusProcess.Arrived)
                return StatusCode(StatusCodes.Status403Forbidden,
                    "Status process may not change, product has already arrived");

            order.StatusProcess = StatusProcess.Arrived;

            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();

            // Send event
            _rmq.Send(new DomainEvent(order, EventType.Updated, "transport_exchange_order", true));

            return StatusCode(StatusCodes.Status202Accepted);
        }
        catch (DbUpdateException)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}