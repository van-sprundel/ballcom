using System.Text.Json;
using EventService;
using EventService.AggregateModels;
using EventService.Data;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

var connStr = builder.Configuration.GetConnectionString("MariaDbConnectionString");
builder.Services.AddDbContext<EventContext>(options =>
    options.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));

var connection = new ConnectionFactory
{
    HostName = "rabbitmq",
    Port = 5672,
    UserName = "Rathalos",
    Password = "1234",
    DispatchConsumersAsync = true
}.CreateConnection();

builder.Services.AddHostedService<EventsReceiver>();
builder.Services.AddSingleton(connection);

var app = builder.Build();

app.MapGet("/", () => "Hello EventWorld from EventService!");

app.MapGet("/order/total-revenue", (EventContext ec) =>
{
    var totalRevenue = 0;

    foreach (var data in ec.Events.Where(x => x.Name == "UpdateOrder").Select(o => o.Data))
    {
        var order = JsonSerializer.Deserialize<Order>(data);
        if (order != null)
        {
            totalRevenue += (int)(order.Price * 100);
        }
    }

    var decimalRevenue = (decimal) totalRevenue / 100;
    return Results.Ok(decimalRevenue);
});

app.Run();