using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceDesk.DataAccess;
using ServiceDesk.Models;

namespace ServiceDesk.Controllers;
    [Route("/api/[controller]")]
    public class ServiceDeskController : Controller
    {
        ServiceDeskDbContext _dbContext;

        public ServiceDeskController(ServiceDeskDbContext dbContext)
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
            var tickets = await _dbContext.Set<Ticket>()
            .Include(c => c.Customer)
            .Select(
                x => new TicketViewModel
                {
                    TicketId = x.TicketId,
                    TicketText = x.TicketText,
                    Status = x.Status,
                    CustomerId = x.CustomerId,
                    Customer = x.Customer
                }).ToListAsync();

            return Ok(tickets);
        }

        [HttpGet]
        [Route("{id}", Name = "GetByTicketId")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var ticket = await _dbContext
                .Set<Ticket>()
                .FirstOrDefaultAsync(c => c.TicketId == id);
                
            if (ticket == null)
            {
                return NotFound();
            }

            return this.Ok(new TicketViewModel
            {
                TicketId = ticket.TicketId,
                TicketText = ticket.TicketText,
                Status = ticket.Status,
                CustomerId = ticket.CustomerId,
                Customer = ticket.Customer
            });
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] TicketCreateViewModel form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var ticket = new Ticket
                    {
                        TicketId = form.TicketId,
                        TicketText = form.TicketText,
                        CustomerId = form.CustomerId,
                        Status = form.Status,
                    };

                    // insert customer
                    await _dbContext
                    .Set<Ticket>()
                    .AddAsync(ticket);
                    
                    await _dbContext.SaveChangesAsync();

                    //TODO: send event

                    // return result
                    return CreatedAtRoute("GetByCustomerId", new { ticketId = ticket.TicketId }, ticket);
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