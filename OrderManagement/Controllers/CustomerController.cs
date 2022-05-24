using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagement.DataAccess;
using OrderManagement.Models;

namespace OrderManagement.Controllers;

[Route("/api/[controller]")]
public class CustomerController : Controller
{
    private readonly OrderManagementDbContext _dbContext;

    public CustomerController(OrderManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [Route("{customerId}")]
    public async Task<IActionResult> GetByCustomerId(int customerId)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
        if (customer == null)
        {
            return StatusCode(StatusCodes.Status404NotFound, "Customer could not be found");
        }
        customer.Orders = _dbContext.Orders.Where(o => o.CustomerId == customerId).ToList();
        return Ok(customer);
    }
}