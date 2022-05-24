using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplierManagement.DataAccess;
using SupplierManagement.Models;

namespace SupplierManagement.Controllers;

[Route("/api/[controller]")]
public class SupplierController : Controller
{
    SupplierManagementDbContext _dbContext;

    public SupplierController(SupplierManagementDbContext dbContext)
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
        return Ok(await _dbContext.Suppliers.ToListAsync());
    }


    [HttpGet]
    [Route("{supplierId}", Name = "GetBySupplierId")]
    public async Task<IActionResult> GetAsync(int id)
    {
        var supplier = await _dbContext
            .Set<Supplier>()
            .FirstOrDefaultAsync(c => c.SupplierId == supplierId);

        if (supplier == null)
        {
            return NotFound();
        }

        return this.Ok(new SupplierViewModel
        {
            SupplierId = supplier.SupplierId,
            Name = supplier.Name,
            Email = supplier.Email
        });
    }

    [HttpPost]
    public async Task<IActionResult> AddSupplierAsync([FromBody] CreateSupplierViewModel form)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var supplier = new Supplier
                {
                    Name = form.Name,
                    Email = form.Email
                };
                _dbContext.Suppliers.Add(supplier);
                await _dbContext.SaveChangesAsync();
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