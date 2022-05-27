using BallCore.Events;
using BallCore.RabbitMq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Controllers;
using OrderManagement.DataAccess;
using OrderManagement.Models;
using OrderManagement.ViewModels;
using RabbitMQ.Client;

namespace OrderManagement;

public class OrderMessageReceiver : MessageReceiver
{
    private readonly OrderManagementDbContext _dbContext;
    private readonly IMessageSender _rmq;

    public OrderMessageReceiver(IConnection connection, OrderManagementDbContext dbContext, IMessageSender rmq) :
        base(connection, new[] { "order_management" })
    {
        _dbContext = dbContext;
        _rmq = rmq;
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
                    Console.WriteLine(
                        $"Received ex: {de.UseExchange} {de.Type} message ({de.Name}) from {de.Destination} : {c.ArrivalAdress}");
                    if (de.Type == EventType.Updated)
                    {
                        // Update het order.
                        var existingOrder = _dbContext.Orders.FirstOrDefault(o => o.OrderId == c.OrderId);

                        if (existingOrder == null) return Task.FromResult(new NotFoundObjectResult("Couldn't find order"));
                        
                        existingOrder.StatusProcess = c.StatusProcess ?? existingOrder.StatusProcess;
                        existingOrder.IsPaid = c.IsPaid;

                        _dbContext.Orders.Update(existingOrder);
                        _dbContext.SaveChanges();

                        _rmq.Send(new DomainEvent(existingOrder, EventType.Updated, "order_exchange_order", true));
                    }

                    break;
                }

                case Customer c:
                {
                    Console.WriteLine(
                        $"Received ex: {de.UseExchange} {de.Type} message ({de.Name}) from {de.Destination} : {c.Email}");
                    if (de.Type == EventType.Created)
                    {
                        var customer = new Customer
                        {
                            CustomerId = c.CustomerId,
                            Email = c.Email
                        };
                        _dbContext.Customers.Add(customer);
                        _dbContext.SaveChanges();
                        break;
                    }

                    if (de.Type == EventType.Deleted)
                    {
                        var customer = _dbContext.Customers.FirstOrDefault(cu => cu.CustomerId == c.CustomerId);
                        if (customer == null) break;
                        
                        _dbContext.Customers.Remove(customer);
                        _dbContext.SaveChanges();
                        break;
                    }

                    if (de.Type == EventType.Updated)
                    {
                        var customer = _dbContext.Customers.FirstOrDefault(cu => cu.CustomerId == c.CustomerId);
                        if (customer == null) break;
                        
                        customer.Email = c.Email;
                        _dbContext.Customers.Update(customer);
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
                        var product = new Product
                        {
                            ProductId = c.ProductId,
                            Name = c.Name,
                            Quantity = c.Quantity,
                            Price = c.Price,
                        };
                        _dbContext.Products.Add(product);
                        _dbContext.SaveChanges();
                        break;
                    }

                    if (de.Type == EventType.Deleted)
                    {
                        // Remove het product
                        var product = _dbContext.Products.FirstOrDefault(p => p.ProductId == c.ProductId);
                        if (product != null)
                        {
                            _dbContext.Products.Remove(product);
                            _dbContext.SaveChanges();
                        }
                        break;
                    }

                    if (de.Type == EventType.Updated)
                    {
                        // Update het product
                        var product = _dbContext.Products.FirstOrDefault(p => p.ProductId == c.ProductId);
                        if (product != null)
                        {
                            product.Name = c.Name;
                            product.Price = c.Price;
                            product.Quantity = c.Quantity;
                            _dbContext.Products.Update(product);
                            _dbContext.SaveChangesAsync();
                        }
                        break;
                    }

                    break;
                }

                case OrderProductViewModel c:
                {
                    Console.WriteLine($"Received ex: {de.UseExchange} {de.Type} message ({de.Name}) from {de.Destination} : {c.OrderProductId}");
                    if (de.Type == EventType.Updated)
                    {
                        // Verander orderstatus.
                        var orderProduct = _dbContext.OrderProduct.FirstOrDefault(op => op.OrderProductId == c.OrderProductId);
                        if (orderProduct != null)
                        {
                            orderProduct.IsPicked = c.IsPicked;
                            _dbContext.OrderProduct.Update(orderProduct);
                            _dbContext.SaveChanges();
                        }
                        break;
                    }
                    break;
                }
            }
        }

        return Task.CompletedTask;
    }
}