using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagement.DataAccess;

namespace OrderManagement.Controllers;

[Route("/api/[controller]")]
public class ProductController : Controller
{
    private readonly OrderManagementDbContext _dbContext;

    public ProductController(OrderManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _dbContext.Products.ToListAsync());
    }

}