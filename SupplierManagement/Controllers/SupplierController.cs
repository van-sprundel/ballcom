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
        var suppliers = await _dbContext.Set<Supplier>().Select(
            x => new SupplierViewModel
            {
                Name = x.Name,
                Email = x.Email,
                Products = x.Products
            }).ToListAsync();

        return Ok(await _dbContext.Suppliers.ToListAsync());
    }


    [HttpGet]
    [Route("{supplierId}", Name = "GetBySupplierId")]
    public async Task<IActionResult> GetAsync(int supplierId)
    {
        var supplier = await _dbContext
            .Set<Supplier>()
            .FirstOrDefaultAsync(c => c.SupplierId == supplierId);

        if (supplier == null)
        {
            return NotFound("Can't find supplier");
        }

        return this.Ok(new SupplierViewModel
        {
            Name = supplier.Name,
            Email = supplier.Email,
            Products = supplier.Products
        });
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddSupplierAsync([FromBody] CreateSupplierViewModel form)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var supplier = new Supplier
                {
                    Name = form.Name,
                    Email = form.Email,
                    Products = new List<Product>()
                };
                _dbContext.Suppliers.Add(supplier);
                await _dbContext.SaveChangesAsync();
                return CreatedAtRoute("GetBySupplierId", new { supplierId = supplier.SupplierId }, supplier);

            }
            else
            {
                return BadRequest();
            }
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Unable to save changes. ");
            return StatusCode(StatusCodes.Status500InternalServerError);
            throw;
        }
    }
}