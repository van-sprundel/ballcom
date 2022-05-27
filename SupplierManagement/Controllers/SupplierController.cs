using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplierManagement.DataAccess;
using SupplierManagement.Models;

namespace SupplierManagement.Controllers;

[Route("/api/[controller]")]
public class SupplierController : Controller
{
    private readonly SupplierManagementDbContext _dbContext;

    public SupplierController(SupplierManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var suppliers = await _dbContext
            .Set<Supplier>()
            .Select(
                x => new SupplierViewModel
                {
                    SupplierId = x.SupplierId,
                    Name = x.Name,
                    Email = x.Email,
                    Products = x.Products
                }).ToListAsync();

        return Ok(suppliers);
    }


    [HttpGet]
    [Route("{id}", Name = "GetBySupplierId")]
    public async Task<IActionResult> GetAsync(int id)
    {
        var supplier = await _dbContext
            .Set<Supplier>()
            .FirstOrDefaultAsync(c => c.SupplierId == id);

        if (supplier == null) 
            return NotFound("Can't find supplier");

        return Ok(new SupplierViewModel
        {
            SupplierId = supplier.SupplierId,
            Name = supplier.Name,
            Email = supplier.Email,
            Products = supplier.Products
        });
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateAsync([FromBody] SupplierCreateForm form)
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

                await _dbContext
                    .Set<Supplier>()
                    .AddAsync(supplier);

                await _dbContext.SaveChangesAsync();

                // return CreatedAtRoute("GetBySupplierId", new { id = supplier.SupplierId }, supplier);
                return StatusCode(201,supplier);
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

    [HttpPost]
    [Route("update")]
    public async Task<IActionResult> UpdateAsync([FromBody] SupplierUpdateForm form)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var supplier = await this._dbContext
                    .Set<Supplier>()
                    .FirstOrDefaultAsync(x => x.SupplierId == form.Id);

                if(supplier == null)
                {
                    return this.NotFound("Supplier not found");
                }

                supplier.Name = form.Name;
                supplier.Email = form.Email;

                _dbContext
                    .Set<Supplier>()
                    .Update(supplier);

                await _dbContext.SaveChangesAsync();

                // return CreatedAtRoute("GetBySupplierId", new { id = supplier.SupplierId }, supplier);
                return Ok(supplier);
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