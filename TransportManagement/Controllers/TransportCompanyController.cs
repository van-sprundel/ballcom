using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportManagement.DataAccess;
using TransportManagement.Models;

namespace TransportManagement.Controllers;

[Route("/api/[controller]")]
public class TransportCompanyController : Controller
{
    private readonly TransportManagementDbContext _dbContext;

    public TransportCompanyController(TransportManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _dbContext.TransportCompanies.ToListAsync());
    }
    
    [HttpGet]
    [Route("{companyId}", Name = "GetByCompanyId")]
    public async Task<IActionResult> GetByCompanyId(int companyId)
    {
        var transportCompany = await _dbContext.TransportCompanies.FirstOrDefaultAsync(t => t.TransportCompanyId == companyId);
        if (transportCompany == null)
        {
            return NotFound();
        }
        return Ok(transportCompany);
    }

    [HttpPost]
    public async Task<IActionResult> AddTransportCompany([FromBody] TransportCompany form)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            TransportCompany transportCompany = new TransportCompany
            {
                Name = form.Name,
                PricePerKm = form.PricePerKm
            };
            // insert transportCompany
            _dbContext.TransportCompanies.Add(transportCompany);
            await _dbContext.SaveChangesAsync();
            
            //TODO: Send event???
            
            //return result
            return CreatedAtRoute("GetByCompanyId", new { companyId = transportCompany.TransportCompanyId },
                transportCompany);
        }
        catch (DbUpdateException)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    

}