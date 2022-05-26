using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BallCore.RabbitMq;
using BallCore.Events;
using InventoryManagement.DataAccess;
using InventoryManagement.Models;

namespace InventoryManagement.Controllers;

[Route("/api/[controller]")]
public class InventoryController : Controller
{
    InventoryManagementDbContext _dbContext;
    IMessageSender _messageSender;

    public InventoryController(InventoryManagementDbContext dbContext, IMessageSender messageSender)
    {
        _dbContext = dbContext;
        _messageSender = messageSender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var products = await _dbContext.Set<Product>().Select(
            x => new ProductViewModel
            {
                Name = x.Name,
                Price = x.Price,
                Quantity = x.Quantity
            }).ToListAsync();

        return this.Ok(products);
    }

    [HttpGet]
    [Route("{productId}", Name = "GetByProductId")]
    public async Task<IActionResult> Get(int productId)
    {
        var product = await _dbContext
            .Set<Product>()
            .FindAsync(productId);

        if (product == null)
        {
            return NotFound("Couldn't find product");
        }

        return this.Ok(new ProductViewModel
        {
            Name = product.Name,
            Price = product.Price,
            Quantity = product.Quantity
        });
    }

    [HttpDelete]
    [Route("delete/{id}", Name = "Delete product")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var product = await _dbContext
            .Set<Product>()
            .FirstOrDefaultAsync(c => c.ProductId == id);

        if (product == null)
        {
            return NotFound("Couldn't find product");
        }
        return this.Ok(product);
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create([FromBody] ProductCreateForm form)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = form.Name,
                    Price = form.Price,
                    Quantity = form.Quantity
                };

                // insert product
                _dbContext
                    .Set<Product>()
                    .Add(product);

                await _dbContext.SaveChangesAsync();

                //TODO: send event
                _messageSender.Send(new DomainEvent(product, EventType.Created, "inventory_exchange", true));

                // return result
                return CreatedAtRoute("GetByProductId", new { customerId = product.ProductId }, product);
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

    [AllowAnonymous]
    [HttpPost]
    [Route("update")]
    public async Task<IActionResult> Update([FromBody] ProductUpdateForm form)
    {
        var product = await this._dbContext.Set<Product>().FirstOrDefaultAsync(x => x.ProductId == form.ProductId);

        if (product == null)
        {
            return this.NotFound();
        }

        //Update product
        product.Quantity = form.Quantity;
        product.Price = form.Price;
        product.Name = form.Name;

        // insert product
        _dbContext
            .Set<Product>()
            .Update(product);

        await _dbContext.SaveChangesAsync();

        //TODO: send event
        _messageSender.Send(new DomainEvent(product, EventType.Updated, "inventory_exchange", true));

        // return result
        return CreatedAtRoute("GetByProductId", new { customerId = product.ProductId }, product);
    }
}