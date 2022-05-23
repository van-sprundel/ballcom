using System;
using System.Linq;
using System.Threading.Tasks;
using BallCore.Model;
﻿using CustomerManagement.Models;
using CustomerManagement.DataAccess;
using CustomerManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Models;
using Customer = BallCore.Model.Customer;

namespace CustomerManagement.Controllers
{
    [Route("/api/[controller]")]
    public class CustomersController : Controller
    {
        CustomerManagementDbContext _dbContext;

        public CustomersController(CustomerManagementDbContext dbContext)
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
            Console.WriteLine("called");
            return Ok(await _dbContext.Customers.ToListAsync());
        }

        [HttpGet]
        [Route("{customerId}", Name = "GetByCustomerId")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            var customer = await _dbContext
                .Set<Customer>()
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
                
            if (customer == null)
            {
                return NotFound();
            }

            return this.Ok(new CustomerViewModel
            {
                Email = customer.Email
            });
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync([FromBody] CreateCustomerViewModel form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var customer = new Customer
                    {
                        Email = form.Email
                    };

                    // insert customer
                    _dbContext.Customers.Add(customer);
                    await _dbContext.SaveChangesAsync();

                    //TODO: sent event

                    // return result
                    return CreatedAtRoute("GetByCustomerId", new { customerId = customer.CustomerId }, customer);
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
}