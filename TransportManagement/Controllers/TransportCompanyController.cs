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

    [HttpGet]
    [Route("lowestPrice", Name = "GetLowestPrice")]
    public async Task<IActionResult> GetLowestPrice()
    {
        var transportCompany = await _dbContext.TransportCompanies.OrderBy(tc => tc.PricePerKm).FirstOrDefaultAsync();
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

            //return result
            return CreatedAtRoute("GetByCompanyId", new { companyId = transportCompany.TransportCompanyId },
                transportCompany);
        }
        catch (DbUpdateException)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut]
    [Route("{companyId}", Name = "EditTransportCompany")]
    public async Task<IActionResult> EditTransportCompany([FromBody] TransportCompany form, int companyId)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingCompany = _dbContext.TransportCompanies.FirstOrDefault(tc => tc.TransportCompanyId == companyId);
            if (existingCompany == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "Transport Company not found.");
            }

            existingCompany.Name = form.Name;
            existingCompany.PricePerKm = form.PricePerKm;

            _dbContext.TransportCompanies.Update(existingCompany);
            await _dbContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status202Accepted);
        }
        catch (DbUpdateException)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete]
    [Route("{companyId:int}", Name = "DeleteTransportCompany")]
    public async Task<IActionResult> DeleteTransportCompany(int companyId)
    {
        try
        {
            var existingCompany = await _dbContext.TransportCompanies.FirstOrDefaultAsync(tc => tc.TransportCompanyId == companyId);
            if (existingCompany == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "Company could not be found");
            }

            _dbContext.TransportCompanies.Remove(existingCompany);
            await _dbContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status204NoContent);

        }
        catch (DbUpdateException )
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    



}