using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BallCore.RabbitMq;
using BallCore.Events;
using ServiceDesk.DataAccess;
using ServiceDesk.Models;

namespace ServiceDesk.Controllers;

[Route("/api/[controller]")]
public class CustomersController : Controller
{
    ServiceDeskDbContext _dbContext;

    public CustomersController(ServiceDeskDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [Route("{customerId}", Name = "GetByCustomerId")]
    public async Task<IActionResult> GetByCustomerId(int customerId)
    {
        var customer = await _dbContext
            .Set<Customer>()
            .FindAsync(customerId);

        if (customer == null)
        {
            return NotFound("Couldn't find customer");
        }

        return Ok(new CustomerViewModel
        {
            Email = customer.Email,
            LastName = customer.LastName
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        var customers = await _dbContext
            .Set<Customer>()
            .Select(x => new CustomerViewModel
            {
                Email = x.Email,
                LastName = x.LastName
            })
            .ToListAsync();

        return Ok(customers);
    }
}