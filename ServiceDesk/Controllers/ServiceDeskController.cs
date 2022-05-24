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

        [HttpDelete]
        [Route("delete/{id}", Name = "Delete Ticket")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var ticket = await _dbContext
                .Set<Ticket>()
                .FirstOrDefaultAsync(c => c.TicketId == id);
                
            if (ticket == null)
            {
                return NotFound();
            }

            this._dbContext.Set<Ticket>().Remove(ticket);
            await _dbContext.SaveChangesAsync();

            return this.Ok();
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] TicketCreateForm form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var ticket = new Ticket
                    {
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
                    return this.Ok(new TicketViewModel 
                    {
                        TicketId = ticket.TicketId,
                        TicketText = ticket.TicketText,
                        Status = ticket.Status,
                        CustomerId = ticket.CustomerId
                    })
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
        public async Task<IActionResult> Update([FromBody] TicketUpdateForm form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var ticket = await this._dbContext
                    .Set<Ticket>()
                    .FirstOrDefaultAsync(x => x.TicketId == form.TicketId);

                    if (ticket == null) {
                        return this.NotFound();
                    }
                    
                    ticket.Status = form.Status;

                    // Check if ticket is solved
                    if(ticket.Status == 2) {
                        //Send message to customer
                    }

                    _dbContext
                    .Set<Ticket>()
                    .Update(ticket);
                    
                    await _dbContext.SaveChangesAsync();

                    //TODO: send event

                    // return result
                    return this.CreatedAtRoute("UpdateTicket", new { ticketId = ticket.TicketId }, ticket);
                }

                return this.BadRequest();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. ");
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
    }