using System.Text.Json;
using BallCore.Events;
using BallCore.RabbitMq;
using CustomerManagement;
using CustomerManagement.DataAccess;
using CustomerManagement.Models;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// add DBContext
var mariaDbConnectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
builder.Services.AddDbContext<CustomerManagementDbContext>(options =>
    options.UseMySql(mariaDbConnectionString, ServerVersion.AutoDetect(mariaDbConnectionString)));

//Inject receiver
builder.Services.AddHostedService<CustomerMessageReceiver>();

//Inject sender
builder.Services.AddTransient<IMessageSender>(_ => new MessageSender(new [] {"payment"},"payment_exchange"));

// Add framework services
builder.Services
    .AddMvc(options => options.EnableEndpointRouting = false);

// setup MVC
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseMvc();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.MapGet("/", () => "Hello World from customermanagement!!!!!");
app.MapGet("/send", (IMessageSender rmq) =>
{
    var customer = new Customer
    {
        CustomerId = -1,
        Email = "email@gmail.com",
        Address = "address",
        City = "city",
        FirstName = "FirstName",
        LastName = "LastName"
    };

    //Send domain event to broker
    rmq.Send(new DomainEvent(customer, EventType.Created, "","customer_exchange"));
    
    Console.WriteLine("Sending message");
    return Results.Ok($"Sent message: {JsonSerializer.Serialize(customer)}");
});

Console.WriteLine("Starting application");
app.Run();