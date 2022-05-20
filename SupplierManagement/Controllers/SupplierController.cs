using BallCore.Model;
using SupplierManagement.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Controllers;

[Route("/api/[controller]")]
public class SupplierController : Controller
{
    SupplierManagementDbContext _dbContext;

    public SupplierController(SupplierManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _dbContext.Suppliers.ToListAsync());
    }

    [HttpGet]
    [Route("{supplierId}", Name = "GetBySupplierId")]
    public async Task<IActionResult> GetBySupplierId(int supplierId)
    {
        var supplier = await _dbContext.Suppliers.FirstOrDefaultAsync(c => c.SupplierId == supplierId);
        if (supplier != null) { return Ok(customer); }
    }
}