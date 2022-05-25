using SupplierManagement.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplierManagement.Models;

namespace ProductManagement.Controllers;

[Route("/api/[controller]")]
public class ProductController : Controller
{
    SupplierManagementDbContext _dbContext;

    public ProductController(SupplierManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _dbContext.Products.ToListAsync());
    }

    [HttpGet]
    [Route("{productId}", Name = "GetByProductId")]
    public async Task<IActionResult> GetByProductId(int productId)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(c => c.ProductId == productId);
        if (product != null)
        {
            return Ok(product);
        }
        else
        {
            return NotFound("Product not found");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddProductAsync([FromBody] CreateProductViewModel form)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = form.Name,
                };
                _dbContext.Products.Add(product);
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