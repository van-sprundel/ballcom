using BallCore.Events;
using BallCore.RabbitMq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplierManagement.DataAccess;
using SupplierManagement.Models;

namespace SupplierManagement.Controllers;

[Route("/api/[controller]")]
public class ProductController : Controller
{
    private readonly SupplierManagementDbContext _dbContext;
    private readonly IMessageSender _messageSender;

    public ProductController(SupplierManagementDbContext dbContext, IMessageSender messageSender)
    {
        _dbContext = dbContext;
        _messageSender = messageSender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var products = await _dbContext
            .Set<Product>()
            .Select(x => new ProductViewModel
            {
                Id = x.ProductId,
                Name = x.Name,
                Supplier = x.Supplier,
            })
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet]
    [Route("{id}", Name = "GetByProductId")]
    public async Task<IActionResult> Get(int id)
    {
        var product = await _dbContext
            .Set<Product>()
            .Select(x => new ProductViewModel
            {
                Name = x.Name,
                Supplier = x.Supplier,
            })
            .FirstAsync();

        if (product == null)
            return NotFound("Product not found");

        return Ok(product);
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateAsync([FromBody] ProductCreateForm form)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var supplier = await this._dbContext
                    .Set<Supplier>()
                    .FirstOrDefaultAsync(x => x.SupplierId == form.SupplierId);

                if(supplier == null)
                {
                    return this.NotFound("Supplier not found");
                }

                var product = new Product
                {
                    Name = form.Name,
                    SupplierId = form.SupplierId,
                };

                // Add product
                await _dbContext
                    .Set<Product>()
                    .AddAsync(product);

                // Save db
                await _dbContext.SaveChangesAsync();

                // Send event
                _messageSender.Send(new DomainEvent(product, EventType.Created, "supplier_exchange",true));

                return StatusCode(201, product);
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
        var product = await _dbContext
            .Set<Product>()
            .FirstOrDefaultAsync(x => x.ProductId == form.Id);

        if (product == null) return NotFound();

        product.Name = form.Name;

        // Add product
        _dbContext.Products.Update(product);

        // Save db
        await _dbContext.SaveChangesAsync();

        // Send event
        _messageSender.Send(new DomainEvent(product, EventType.Updated, "inventory_management"));

        return Ok(product);
    }
}