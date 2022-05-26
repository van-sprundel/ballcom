using SupplierManagement.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplierManagement.Models;
using BallCore.RabbitMq;
using BallCore.Events;

namespace ProductManagement.Controllers;

[Route("/api/[controller]")]
public class ProductController : Controller
{
    SupplierManagementDbContext _dbContext;
    IMessageSender _messageSender;

    public ProductController(SupplierManagementDbContext dbContext, IMessageSender messageSender)
    {
        _dbContext = dbContext;
        _messageSender = messageSender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _dbContext.Products.ToListAsync());
    }

    [HttpGet]
    [Route("{productId}", Name = "GetByProductId")]
    public async Task<IActionResult> Get(int productId)
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
    [Route("create")]
    public async Task<IActionResult> CreateAsync([FromBody] ProductCreateForm form)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = form.Name,
                    SupplierId = form.SupplierId
                };

                // Add product
                await _dbContext.Products.AddAsync(product);

                // Save db
                await _dbContext.SaveChangesAsync();

                // Send event
                this._messageSender.Send(new DomainEvent(product, EventType.Created, "inventory_management", false)); 
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
    public async Task<IActionResult> UpdateAsync([FromBody] ProductUpdateForm form)
    {
        var product = await this._dbContext.Set<Product>().FirstOrDefaultAsync(x => x.ProductId == form.Id);

        if (product == null)
        {
            return this.NotFound();
        }

        product.Name = form.Name;

        // Add product
        _dbContext.Products.Update(product);

        // Save db
        await _dbContext.SaveChangesAsync();

        // Send event
        this._messageSender.Send(new DomainEvent(product, EventType.Updated, "inventory_management", false));

        return this.Ok();
    }
}