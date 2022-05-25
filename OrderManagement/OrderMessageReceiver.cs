using BallCore.Events;
using BallCore.RabbitMq;
using Microsoft.EntityFrameworkCore;
using OrderManagement.DataAccess;
using OrderManagement.Models;
using RabbitMQ.Client;

namespace OrderManagement;

public class OrderMessageReceiver : MessageReceiver
{
    
    private readonly OrderManagementDbContext _dbContext;
    
    public OrderMessageReceiver(IConnection connection, OrderManagementDbContext dbContext) : base(connection, new[] {"customer", "product", "order", "general"})
    {
        _dbContext = dbContext;
    }

    protected override Task HandleMessage(IEvent e)
    {
        Console.WriteLine("Received message");
        if (e is DomainEvent de)
        {
            switch (de.Payload)
            {
                case Order c:
                {
                    Console.WriteLine($"Received ex: {de.UseExchange} {de.Type} message ({de.Name}) from {de.Destination} : {c.ArrivalAdress}");
                    if (de.Type == EventType.Updated)
                    {
                        // Update het order.
                        var existingOrder = _dbContext.Orders.FirstOrDefault(o => o.OrderId == c.OrderId);
                        existingOrder.StatusProcess = c.StatusProcess ?? existingOrder.StatusProcess;

                        _dbContext.Orders.Update(existingOrder);
                        _dbContext.SaveChanges();
                        break;
                    }
                    break;
                }
                
                case Customer c:
                {
                    Console.WriteLine($"Received ex: {de.UseExchange} {de.Type} message ({de.Name}) from {de.Destination} : {c.Email}");
                    if (de.Type == EventType.Created)
                    {
                        _dbContext.Customers.Add(c);
                        _dbContext.SaveChanges();
                        break;
                    }

                    if (de.Type == EventType.Deleted)
                    {
                        _dbContext.Customers.Remove(c);
                        _dbContext.SaveChanges();
                        break;
                    }

                    if (de.Type == EventType.Updated)
                    {
                        _dbContext.Customers.Update(c);
                        _dbContext.SaveChanges();
                        break;
                    }
                    break;
                }
                
                case Product c:
                {
                    Console.WriteLine($"Received ex: {de.UseExchange} {de.Type} message ({de.Name}) from {de.Destination} : {c.Name}");
                    if (de.Type == EventType.Created)
                    {
                        // Add het product toe.
                        _dbContext.Products.Add(c);
                        _dbContext.SaveChanges();
                        break;
                    }

                    if (de.Type == EventType.Deleted)
                    {
                        // Remove het product
                        _dbContext.Products.Remove(c);
                        _dbContext.SaveChanges();
                        break;
                    }

                    if (de.Type == EventType.Updated)
                    {
                        // Update het product
                        _dbContext.Products.Update(c);
                        _dbContext.SaveChangesAsync();
                        break;
                    }
                    break;
                }
            }
        }
        
        return Task.CompletedTask;
    }
}