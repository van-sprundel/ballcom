using BallCore.Model;
using CustomerManagement.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Controllers;

[Route("/api/[controller]")]
public class CustomersController : Controller
{
    CustomerManagementDbContext _dbContext;

    public CustomersController(CustomerManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [Route("test", Name = "Test")]
    public async Task<IActionResult> Test()
    {
        return await Task.FromResult(Ok("test"));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        Console.WriteLine("called");
        return Ok(await _dbContext.Customers.ToListAsync());
    }

    [HttpGet]
    [Route("{customerId}", Name = "GetByCustomerId")]
    public async Task<IActionResult> GetByCustomerId(int customerId)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> RegisterAsync([FromBody] Customer customer)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // insert customer
                _dbContext.Customers.Add(customer);
                await _dbContext.SaveChangesAsync();

                //TODO: sent event

                // return result
                return CreatedAtRoute("GetByCustomerId", new { customerId = customer.CustomerId }, customer);
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
}