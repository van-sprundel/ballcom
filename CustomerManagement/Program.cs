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
    var customer = new Customer()
    {
        CustomerId = -1,
        Email = "email@gmail.com",
        Address = "address",
        City = "city",
        FirstName = "FirstName",
        LastNmae = "LastName"
    };

    rmq.Send("general", customer);
    Console.WriteLine("Sending message");
    return Results.Ok($"Sent message: {JsonSerializer.Serialize(customer)}");
});

Console.WriteLine("Starting application");
app.Run();