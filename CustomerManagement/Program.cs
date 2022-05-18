using System.Text.Json;
using BallCore.RabbitMq;
using CustomerManagement;
using CustomerManagement.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// add DBContext
var mariaDbConnectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
builder.Services.AddDbContext<CustomerManagementDbContext>(options =>
    options.UseMySql(mariaDbConnectionString, ServerVersion.AutoDetect(mariaDbConnectionString)));

builder.Services.AddHostedService<CustomerMessageReceiver>();
builder.Services.AddSingleton<IMessageSender>(new MessageSender("general"));

var app = builder.Build();

app.MapGet("/", () => "Hello World from customermanagement!");
app.MapGet("/send", (IMessageSender rmq) =>
{
    var message = new
    {
        Msg = "Hello world",
        Time = DateTime.Now.ToLongTimeString()
    };
    
    rmq.Send("general", message);
    Console.WriteLine("Sending message");
    return Results.Ok($"Sent message: {JsonSerializer.Serialize(message)}");
});

Console.WriteLine("Starting application");
app.Run();